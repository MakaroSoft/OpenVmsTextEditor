package makarosoft.vmsExplorer.controllers;

import java.io.IOException;
import java.util.List;

import com.google.gson.Gson;

import makarosoft.VmsWeb.ApiController;
import makarosoft.VmsWeb.Request;
import makarosoft.VmsWeb.Response;
import makarosoft.vmsExplorer.Directory.DirectoryFactory;
import makarosoft.vmsExplorer.Directory.IDirectory;

public class DirectoryController extends ApiController {
	
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
			System.out.println(e.getMessage());
			e.printStackTrace();
			response.setResponseCode(500, "Internal Server Error");
		}
	}
	
	private List<makarosoft.vmsExplorer.models.File> getFolderContent(String folder, String filter) {
		IDirectory directory = new DirectoryFactory().Create(folder);
		return directory.getFiles(filter);		
	}
}
