package makarosoft.vmsExplorer.Directory;

import java.io.File;
import java.io.FilenameFilter;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class WindowsDirectory implements IDirectory {
	private String _folder;

	public WindowsDirectory(String folder) {
		_folder = folder;
	}

	@Override
	public List<makarosoft.vmsExplorer.models.File> getFiles(String filter) {
		String folder = FileFormatter.toWindowsFolderFormat(_folder);
		
		System.out.println("Filter = " + filter);
		System.out.println("attempting to get folder content for: "+folder);
		
		File theFolder = new File(folder);
		if (!theFolder.exists() || !theFolder.isDirectory()) {
			return null;
		}
		
		ArrayList<makarosoft.vmsExplorer.models.File> directoriesResult = new ArrayList<makarosoft.vmsExplorer.models.File>();
		ArrayList<makarosoft.vmsExplorer.models.File> filesResult = new ArrayList<makarosoft.vmsExplorer.models.File>();
		
		for (File file : theFolder.listFiles()) {
			boolean isDirectory = file.isDirectory();
			
			makarosoft.vmsExplorer.models.File f = new makarosoft.vmsExplorer.models.File();
			f.Name = file.getName();
			f.Dir = isDirectory? isDirectory: null;
			//f.Mod = System.currentTimeMillis();
			
			
			ArrayList<makarosoft.vmsExplorer.models.File> result = isDirectory? directoriesResult : filesResult;
			result.add(f);
		}
		
		List<makarosoft.vmsExplorer.models.File> newList = Stream.concat(directoriesResult.stream(), filesResult.stream())
                .collect(Collectors.toList());
		
		System.out.println("**** Count is: "+newList.size());
		return newList;
	}
}
