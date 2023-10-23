package makarosoft.vmsExplorer.controllers;

import java.io.IOException;
import java.util.List;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.google.gson.Gson;

import makarosoft.VmsWeb.ApiController;
import makarosoft.VmsWeb.Request;
import makarosoft.VmsWeb.Response;
import makarosoft.vmsExplorer.Engine;
import makarosoft.vmsExplorer.Directory.DirectoryFactory;
import makarosoft.vmsExplorer.Directory.IDirectory;

public class DirectoryController extends ApiController {
	
	Logger _logger = LogManager.getLogger(DirectoryController.class);
	
	@Override
	public void get(Request request, Response response) throws IOException {
		try {
			String folder = getRelativePath(request);
			if (folder == null) {
				response.setResponseCode(404, "Not Found");
				return;
			}
			
			String filter = request.getParameter("filter");
			
			List<makarosoft.vmsExplorer.models.File> folderContents = getFolderContent(folder, filter);
			if (folderContents == null) {
				response.setResponseCode(404, "Not Found");
				return;
			}

			Gson gson = new Gson();
			String json = gson.toJson(folderContents);
			
			response.setResponseCode(200, "OK");
			response.addHeader("Content-Type", "application/json");
			response.addBody(json);
		} catch (Exception e) {
			_logger.error(e.getMessage(), e);
			response.setResponseCode(500, "Internal Server Error");
		}
	}
	
	private List<makarosoft.vmsExplorer.models.File> getFolderContent(String folder, String filter) {
		IDirectory directory = new DirectoryFactory().Create(folder);
		return directory.getFiles(filter);		
	}
}
