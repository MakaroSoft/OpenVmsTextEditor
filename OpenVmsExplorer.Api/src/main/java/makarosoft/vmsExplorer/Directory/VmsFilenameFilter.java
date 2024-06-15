package makarosoft.vmsExplorer.Directory;

import java.io.File;
import java.io.FilenameFilter;
import java.util.HashMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import makarosoft.vmsExplorer.util.StrU;

public class VmsFilenameFilter implements FilenameFilter {
	private HashMap<String,String> _doneAlready = new HashMap<String, String>();
	private boolean _currentVersionOnly = false;
	
	private Pattern _includePattern = null;
	private Pattern _excludePattern = null;
	private boolean _ignoreDirectories;
	Logger _logger = LogManager.getLogger(VmsFilenameFilter.class);

	public VmsFilenameFilter(String include, String exclude, boolean showHistory, boolean ignoreDirectories) {
		_doneAlready.clear();
		_ignoreDirectories = ignoreDirectories;
		
		_currentVersionOnly = !showHistory;
		
		// convert the vms include filter to a regular expression
		_includePattern = buildPattern(include);
		
		// convert the vms exclude filter to a regular expression
		_excludePattern = buildPattern(exclude);		
	}
	
	private Pattern buildPattern(String filter) {
		if (isNullOrWhiteSpace(filter)) return null;
		
		String anyCharacter = "[a-z0-9_\\-\\$]";
		String anyNumber = "[0-9]";
				
		String[] filters = filter.split(",");
		 
        for (int i = 0; i < filters.length; i++) {
            String pattern = filters[i];
            
            // example: baie
            if (!pattern.contains(".") && !pattern.contains("*")) pattern += "*.*;*";
            // example: baie*a*
            if (!pattern.contains(".")) pattern += ".*;*";

            if (pattern.endsWith(";")) pattern += "*";
            if (!pattern.contains(";")) pattern += ";*";

            pattern = pattern.replace(".", "\\.");
            pattern = pattern.replace("$", "\\$");

            pattern = pattern.replace(";*", ";" + anyNumber + "+"); // one or more
            pattern = pattern.replace("*", anyCharacter + "*"); // zero or more
            pattern = pattern.replace("?", anyCharacter);
            //pattern = pattern.replace("+", "*"); // change one or more to zero or more

            pattern = "^" + pattern + "$";
            
            filters[i] = pattern;
        }

        // Join the individual patterns with the | (OR) operator
        String finalPattern = String.join("|", filters);
		_logger.debug("Regex filter = {}", finalPattern);
		 
        return Pattern.compile(finalPattern, Pattern.CASE_INSENSITIVE); 		
	}
	
	public VmsFilenameFilter(String include, String exclude, boolean showHistory) {
		this(include, exclude, showHistory, false);
	}

	@Override
	public boolean accept(File dir, String name) {
    	// take all directories unless I am told to ignore them
    	if (StrU.containsIgnoreCase(name, ".DIR;")) {
    		return !_ignoreDirectories;
    	}
    	
    	if (_excludePattern != null) {
    	    Matcher matcher = _excludePattern.matcher(name);
    	    boolean found = matcher.find();
    	    if (found) return false;
    	}
    	
    	if (_includePattern != null) {
    	    Matcher matcher = _includePattern.matcher(name);
    	    boolean found = matcher.find();
    	    if (!found) return false;
    	}
	    return stripDuplicates(name);
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

    private static boolean isNullOrWhiteSpace(String str) {
        return str == null || str.trim().isEmpty();
    }
}
