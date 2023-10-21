package makarosoft.vmsExplorer.Directory;

public class DirectoryFactory {
	public IDirectory Create(String folder) {
		boolean isVms = false;
		int firstSlash = folder.indexOf("/");
		if (firstSlash == -1 && folder.length() != 1) {
				isVms = true; // root folder is larger than one character
		} else {
			if (firstSlash != -1 && firstSlash != 1) {
				isVms = true;
			}
		}
		if (isVms) {
			return new VmsDirectory(folder);
		} else {
			return new WindowsDirectory(folder);
		}
	}
}
