package makarosoft.vmsExplorer.Directory;

import java.util.List;

import makarosoft.vmsExplorer.models.File;

public interface IDirectory {
	List<File> getFiles(String include, String exclude, boolean showHistory);
}
