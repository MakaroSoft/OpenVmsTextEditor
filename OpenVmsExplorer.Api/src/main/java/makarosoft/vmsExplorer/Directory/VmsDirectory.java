package makarosoft.vmsExplorer.Directory;

import java.io.File;
import java.io.FilenameFilter;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import makarosoft.vmsExplorer.util.StrU;

public class VmsDirectory implements IDirectory {
	private String _folder;

	public VmsDirectory(String folder) {
		_folder = folder;
	}

	@Override
	public List<makarosoft.vmsExplorer.models.File> getFiles(String filter) {
		String folder = FileFormatter.toVmsFolderFormat(_folder);
		
		System.out.println("Filter = " + filter);
		VmsFilenameFilter filenameFilter = new VmsFilenameFilter(filter);
		System.out.println("attempting to get folder content for: " + folder);
		
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
		
		System.out.println("**** Count is: "+newList.size());
		return newList;
	}

	private String removeExtension(String name) {
		int index = name.indexOf(".");
		return name.substring(0, index);
	}
}
