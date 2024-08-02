package makarosoft.vmsExplorer.Directory;

import java.io.File;
import java.io.FilenameFilter;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import makarosoft.vmsExplorer.controllers.FileController;
import makarosoft.vmsExplorer.util.StrU;

public class VmsDirectory implements IDirectory {
	private String _folder;
	Logger _logger = LogManager.getLogger(VmsDirectory.class);

	public VmsDirectory(String folder) {
		_folder = folder;
	}

	@Override
	public List<makarosoft.vmsExplorer.models.File> getFiles(String include, String exclude, boolean showHistory) {
		String folder = FileFormatter.toVmsFolderFormat(_folder);
		
		_logger.debug("include = {}, exclude = {}, showHistory = {}", include, exclude, showHistory);
		VmsFilenameFilter filenameFilter = new VmsFilenameFilter(include, exclude, showHistory);
		_logger.debug("attempting to get folder content for: {}", folder);
		
		File theFolder = new File(folder);
		if (!theFolder.exists() || !theFolder.isDirectory()) {
			return null;
		}
				
        // filter by name
        File[] files = theFolder.listFiles(filenameFilter);
		        
		ArrayList<makarosoft.vmsExplorer.models.File> directoriesResult = new ArrayList<makarosoft.vmsExplorer.models.File>();
		ArrayList<makarosoft.vmsExplorer.models.File> filesResult = new ArrayList<makarosoft.vmsExplorer.models.File>();
		
		for (File file : files) {
			String name = file.getName();
			boolean isDirectory = StrU.containsIgnoreCase(name, ".DIR;");
			
			makarosoft.vmsExplorer.models.File f = new makarosoft.vmsExplorer.models.File();
			f.Name = isDirectory? removeExtension(name) : name;
			f.Dir = isDirectory? isDirectory: null;
			
			ArrayList<makarosoft.vmsExplorer.models.File> result = isDirectory? directoriesResult : filesResult;
			
			result.add(f);
		}
		
		List<makarosoft.vmsExplorer.models.File> newList = Stream.concat(directoriesResult.stream(), filesResult.stream())
                .collect(Collectors.toList());
		
		_logger.debug("**** Count is: {}", newList.size());
		return newList;
	}

	private String removeExtension(String name) {
		int index = name.indexOf(".");
		return name.substring(0, index);
	}
}
