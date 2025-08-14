package makarosoft.vmsExplorer.Directory;

public class DirectoryFactory {
	public IDirectory Create(String folder) {
		boolean isVms;
		int colon = folder.indexOf(":");
		// Windows: single-letter drive followed by : or :/
		if (colon == 1) {
			isVms = false;
		} else {
			isVms = true;
		}
		if (isVms) {
			return new VmsDirectory(folder);
		} else {
			return new WindowsDirectory(folder);
		}
	}
}
