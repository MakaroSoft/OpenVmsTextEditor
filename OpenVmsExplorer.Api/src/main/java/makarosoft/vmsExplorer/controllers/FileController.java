package makarosoft.vmsExplorer.controllers;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.stream.Stream;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import makarosoft.VmsWeb.ApiController;
import makarosoft.VmsWeb.Request;
import makarosoft.VmsWeb.Response;
import makarosoft.vmsExplorer.Directory.FileFormatter;
import makarosoft.vmsExplorer.Directory.VmsFilenameFilter;

public class FileController extends ApiController {

	Logger _logger = LogManager.getLogger(FileController.class);
	
	@Override
	public void get(Request request, Response response) throws IOException {
		try {
			String fullName = getRelativePath(request);
			if (fullName == null) {
				response.setResponseCode(404, "Not Found");
				return;
			}
			
			String encoding = FileFormatter.IsVmsFile(fullName)? "charset=iso-8859-1" : "charset=utf-8";
			
			String fileContent = getFile(fullName);
			if (fileContent == null) {
				response.setResponseCode(404, "Not Found");
				return;
			}

			response.setResponseCode(200, "OK");
			response.addHeader("Content-Type", "text/plain; "+encoding);
			response.addBody(fileContent);
		} catch (Exception e) {
			_logger.error(e.getMessage(), e);
			response.setResponseCode(500, "Internal Server Error");
		}
	}

	@Override
	public void post(Request request, Response response) throws IOException {
		try {
			String fullName = getRelativePath(request);
			if (fullName == null) {
				response.setResponseCode(404, "Not Found");
				return;
			}
			
			String encoding = FileFormatter.IsVmsFile(fullName)? "charset=iso-8859-1" : "charset=utf-8";
			String newVersion = saveFile(fullName, request.getBody());
			
			response.setResponseCode(200, "OK");
			response.addHeader("Content-Type", "text/plain; "+encoding);
			response.addBody(newVersion);
		} catch (Exception e) {
			_logger.error(e.getMessage(), e);
			response.setResponseCode(500, "Internal Server Error");
		}
	}
	
	private String saveFile(String fullName, String body) throws IOException {
		
		
		// when getting a file, there will always be at least one slash
		String fullNameFormatted = FileFormatter.IsVmsFile(fullName) ? FileFormatter.toVmsFileFormat(fullName) : FileFormatter.toWindowsFileFormat(fullName);
		_logger.debug("File to save: {}", fullNameFormatted);

		boolean isVms = false;
		int version = 0;
		int index = fullNameFormatted.indexOf(";");
		if (index != -1) {
			isVms = true;
			version = Integer.parseInt(fullNameFormatted.substring(index + 1));
			fullNameFormatted = fullNameFormatted.substring(0, index);
		}
		_logger.debug("Version = {}", version);
				
		byte[] bytes = isVms? body.getBytes("iso-8859-1") : body.getBytes("utf-8");
		
		// I found a bug writing out a new file version. A new file would be okay because it has file attributes given by java. A
		//  new version is not okay because it keeps the file attributes of the original file(would have been okay if that original file was
		//  created by java)
		// The solution is to either create a temporary file name and then rename it later or write out 100% of the bytes at once.
		// I chose to write out 100% of the bytes at once for now.
		FileOutputStream fos = new FileOutputStream(new File(fullNameFormatted));
	    fos.write(bytes); // write out all the bytes at once!
	    fos.flush();
		fos.close();
		
		// a vms thing
		if (version != 0) {
			index = fullNameFormatted.indexOf("]");
			String path = fullNameFormatted.substring(0, index + 1);
			String fileName = fullNameFormatted.substring(index + 1);
			_logger.debug("Path = {}, filename = {}", path, fileName);
			
			VmsFilenameFilter filenameFilter = new VmsFilenameFilter(fileName + ";", true);
			
			File theFolder = new File(path);
					
	        // filter by name
	        File[] files = theFolder.listFiles(filenameFilter);
	        _logger.debug("filename count = {}", files.length);
	        _logger.debug("The new filename is {}", files[0].getName());
	        return files[0].getName();
		}
		
		return "";
		
	}
	
	private String getFile(String fullName) {
		// when getting a file, there will always be at least one slash
		String fullNameFormatted = FileFormatter.IsVmsFile(fullName) ? FileFormatter.toVmsFileFormat(fullName) : FileFormatter.toWindowsFileFormat(fullName);
		_logger.debug("File to download: {}", fullNameFormatted);
		
		if (FileFormatter.IsVmsFile(fullName)) {
			return readLineByLineJava8(fullNameFormatted, StandardCharsets.ISO_8859_1);
		} else {
			return readLineByLineJava8(fullNameFormatted, StandardCharsets.UTF_8);			
		}
		
		
	}
	
	private static String readLineByLineJava8(String filePath, Charset charset) 
    {
        StringBuilder contentBuilder = new StringBuilder();
 
        try (Stream<String> stream = Files.lines( Paths.get(filePath), charset)) 
        {
            stream.forEach(s -> contentBuilder.append(s).append("\r\n"));
        }
        catch (IOException e) 
        {
            e.printStackTrace();
            return null;
        }
 
        if (contentBuilder.length() >= 2) {
            contentBuilder.setLength(contentBuilder.length()-2);        	
        }
        return contentBuilder.toString();
    }	
}	

