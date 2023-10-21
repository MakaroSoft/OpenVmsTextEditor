package makarosoft.vmsExplorer.Directory;

import java.io.File;
import java.io.FilenameFilter;
import java.util.HashMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import makarosoft.vmsExplorer.util.StrU;

public class VmsFilenameFilter implements FilenameFilter {
	private HashMap<String,String> _doneAlready = new HashMap<String, String>();
	private boolean _currentVersionOnly = false;
	
	private Pattern _pattern = null;
	private boolean _ignoreDirectories;

	public VmsFilenameFilter(String filter, boolean ignoreDirectories) {
		_doneAlready.clear();
		_ignoreDirectories = ignoreDirectories;
		
		String anyCharacter = "[a-z0-9_\\-\\$]";
		String anyNumber = "[0-9]";
		
		// convert the vms filter to a regular expression
		if (filter != null) {
			if (filter.endsWith(";") || !filter.contains(";")) _currentVersionOnly = true;
			
			// make sure version is on the filter
			if (filter.endsWith(";")) filter += "*";
			if (!filter.contains(";")) filter += ";*";
			
			filter = filter.replace(".", "\\.");
			filter = filter.replace(";*", ";" + anyNumber + "+");
			filter = filter.replace("*", anyCharacter + "+");
			filter = filter.replace("?", anyCharacter);
			
			filter = "^" + filter + "$";
			
			_pattern = Pattern.compile(filter, Pattern.CASE_INSENSITIVE);
			System.out.println("Regex Filter = " + filter);
		}
		
	}
	
	public VmsFilenameFilter(String filter) {
		this(filter, false);
	}

	@Override
	public boolean accept(File dir, String name) {
    	// take all directories unless I am told to ignore them
    	if (StrU.containsIgnoreCase(name, ".DIR;")) {
    		return !_ignoreDirectories;
    	}
    	
    	if (_pattern != null) {
    	    Matcher matcher = _pattern.matcher(name);
    	    boolean found = matcher.find();
    	    if (!found) return false;
    	    return stripDuplicates(name);
    	}
    	return true;
	}
	
	private boolean stripDuplicates(String name) {
		// we want all files
		if (!_currentVersionOnly) return true;
		
		name = removeVersion(name).toLowerCase();
		if (_doneAlready.containsKey(name)) return false;
		
		_doneAlready.put(name, "");
		return true;
	}

	private String removeVersion(String name) {
		int index = name.indexOf(";");
		return name.substring(0, index);
	}

}
