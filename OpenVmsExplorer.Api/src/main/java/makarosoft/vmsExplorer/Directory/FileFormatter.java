package makarosoft.vmsExplorer.Directory;

import java.io.File;

public class FileFormatter {
	public static String toVmsFolderFormat(String folder) {
		int firstSlash = folder.indexOf("/");
		if (firstSlash == -1) {
			folder = folder + ":[000000]";
		} else {
			folder = folder.replaceFirst("/", ":[");
			folder = folder.replace("/", ".");
			folder = folder + "]";
		}
		return folder;
	}
	public static String toWindowsFolderFormat(String folder) {
		int firstSlash = folder.indexOf("/");
		if (firstSlash == -1) {
				folder = folder + ":" + File.separator;
		} else {
			if (firstSlash == 1) {
				// we are windows
				folder = folder.replaceFirst("/", ":\\\\");
				folder = folder.replace("/", "\\\\");
			}
		}
		return folder;
	}
	public static String toWindowsFolderFormat2(String folder) {
		int firstSlash = folder.indexOf("/");
		if (firstSlash == -1) {
				folder = folder + ":" + File.separator;
		} else {
			if (firstSlash == 1) {
				// we are windows
				folder = folder.replaceFirst("/", ":\\\\");
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
		int firstSlash = fullName.indexOf("/");
		return firstSlash != 1;
	}
}
