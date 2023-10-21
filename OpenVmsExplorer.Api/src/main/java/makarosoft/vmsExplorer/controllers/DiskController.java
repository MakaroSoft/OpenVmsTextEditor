package makarosoft.vmsExplorer.controllers;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;

import javax.swing.filechooser.FileSystemView;

import com.google.gson.Gson;

import makarosoft.VmsWeb.ApiController;
import makarosoft.VmsWeb.Request;
import makarosoft.VmsWeb.Response;

public class DiskController extends ApiController {

	@Override
	public void get(Request request, Response response) throws IOException {
		try {
			if (getRelativePath(request) != null) {
				// we don't support this at the moment
				response.setResponseCode(404, "Not Found");
				return;
			}
			
			String name = null;
			StringBuilder sb = new StringBuilder();
			if (name == null) {
				ArrayList<String> disks = getDisks();

				Gson gson = new Gson();
				String json = gson.toJson(disks);
				sb.append(json);

			} else {
				// TODO to handle other shit
				sb.append("[]");
			}

			response.setResponseCode(200, "OK");
			response.addHeader("Content-Type", "application/json");
			response.addBody(sb.toString());
		} catch (Exception e) {
			System.out.println(e.getMessage());
			e.printStackTrace();
			response.setResponseCode(500, "Internal Server Error");
		}
	}

	private ArrayList<String> getDisks() {
		File[] paths;

		ArrayList<String> disks = new ArrayList<String>();
		// returns pathnames for files and directory
		paths = File.listRoots();

		// for each pathname in pathname array
		for (File path : paths) {
			String pathName = parse(path.getAbsolutePath());
			disks.add(pathName);
		}
		return disks;
	}

	private String parse(String text) {
		int index = text.indexOf(":");
		if (index != -1)
			text = text.substring(0, index);
		if (text.startsWith("/")) text = text.substring(1); // VMS Disks start with a slash for some reason
		return text;
	}
}
