package makarosoft.VmsWeb;

import java.io.IOException;
import java.util.Map;
import java.io.BufferedReader;
import java.io.OutputStream;

/**
 * Handlers must be thread safe.
 */
public abstract class ApiController {
	
	private String _path;

	public String getPath() {
		return _path;
	}
	protected void setPath(String value) {
		_path = value;
	}
	
	public String getRelativePath(Request request) {
		String path = request.getPath();
		if (path.length() == _path.length()) return null;
		return path.substring(_path.length()+1); // don't include the slash
	}
	
	public void get(Request request, Response response) throws IOException {
		response.setResponseCode(404, "Not Found");		
	}
	
	public void post(Request request, Response response) throws IOException {
		response.setResponseCode(404, "Not Found");
	}
	public void put(Request request, Response response) throws IOException {
		response.setResponseCode(404, "Not Found");
	}
	public void delete(Request request, Response response) throws IOException {
		response.setResponseCode(404, "Not Found");
	}
}