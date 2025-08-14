package makarosoft.vmsExplorer.Directory;

import java.io.File;

public class FileFormatter {
    public static String toVmsFolderFormat(String folder) {
        int firstSlash = folder.indexOf("/");
        if (firstSlash == -1) {
            // "disk:" should map to disk:[000000]
            folder = folder + "[000000]";
        } else {
            // Expecting "disk:/path1/path2" → "disk:[path1.path2]"
            folder = folder.replaceFirst(":/", ":[");
            folder = folder.replace("/", ".");
            folder = folder + "]";
        }
        return folder;
    }
    public static String toWindowsFolderFormat(String folder) {
        int firstSlash = folder.indexOf("/");
        if (firstSlash == -1) {
            // "C:" → "C:\\"
            folder = folder + File.separator;
        } else {
            // Expecting "C:/path1/path2"; ensure we translate from drive:/ to Windows
            // Only treat as Windows if drive token is 1 char before ':'
            int colon = folder.indexOf(":");
            if (colon == 1) {
                folder = folder.replaceFirst(":/", ":\\\\");
                folder = folder.replace("/", "\\\\");
            }
        }
        return folder;
    }
    public static String toWindowsFolderFormat2(String folder) {
        int firstSlash = folder.indexOf("/");
        if (firstSlash == -1) {
            folder = folder + File.separator;
        } else {
            int colon = folder.indexOf(":");
            if (colon == 1) {
                folder = folder.replaceFirst(":/", ":\\\\");
                folder = folder.replace("/", "\\");
            }
        }
        return folder;
    }
	
	public static String toVmsFileFormat(String fullName) {
		if (fullName.endsWith("/")) return null;
		
		int index = fullName.lastIndexOf("/");
		if (index == 0) return null;
		
		String folder = fullName.substring(0,index);
		String fileName = fullName.substring(index + 1);
		
		return FileFormatter.toVmsFolderFormat(folder) + fileName;
	}

	public static String toWindowsFileFormat(String fullName) {
		if (fullName.endsWith("/")) return null;
		
		int index = fullName.lastIndexOf("/");
		if (index == 0) return null;
		
		String folder = fullName.substring(0,index);
		String fileName = fullName.substring(index + 1);
		
        return FileFormatter.toWindowsFolderFormat2(folder) + "\\" + fileName;
	}
	
	public static boolean IsVmsFile(String fullName) {
        // Windows: single-letter drive followed by :/ (e.g., C:/...)
        int colon = fullName.indexOf(":");
        if (colon == 1 && fullName.length() > 2 && fullName.charAt(2) == '/') return false;
        // Otherwise treat as VMS
        return true;
	}
}
