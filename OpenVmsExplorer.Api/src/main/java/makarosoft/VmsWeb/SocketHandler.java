package makarosoft.VmsWeb;
import java.io.IOException;
import java.util.ArrayList;
import java.net.Socket;
import java.io.OutputStream;
import java.io.InputStreamReader;
import java.io.BufferedReader;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import makarosoft.vmsExplorer.Engine;


class SocketHandler implements Runnable {
	private Socket _socket;
	private ArrayList<ApiController> _controllers;
	Logger _logger = LogManager.getLogger(SocketHandler.class);

	public SocketHandler(Socket socket, ArrayList<ApiController> controllers) {
		_socket = socket;
		_controllers = controllers;
	}

	/**
	 * Simple responses like errors. Normal reponses come from handlers.
	 */
	private void respond(int statusCode, String msg, OutputStream out) throws IOException {
		String responseLine = "HTTP/1.1 " + statusCode + " " + msg + "\r\n\r\n";
		_logger.debug(responseLine);
		out.write(responseLine.getBytes());
	}

	public void run() {
		BufferedReader in = null;
		OutputStream out = null;

		try {
			in = new BufferedReader(new InputStreamReader(_socket.getInputStream()));
			out = _socket.getOutputStream();

			Request request = new Request(in);
			if (!request.parse()) {
				respond(500, "Unable to parse request", out);
				return;
			}

			boolean foundHandler = false;
			Response response = new Response(out);
			
			for (ApiController apiController : _controllers) {
				String path = fixPath(apiController.getPath());
				Pattern pattern = Pattern.compile("^" + path + "(/[a-z0-9_;\\-\\$\\.]+)*$", Pattern.CASE_INSENSITIVE);
			    Matcher matcher = pattern.matcher(request.getPath());
			    boolean matchFound = matcher.find();
				if (matchFound) {
					switch (request.getMethod()) {
					case "GET":
						apiController.get(request, response);
						break;
					case "POST":
						apiController.post(request, response);
						break;
					case "PUT":
						apiController.put(request, response);
						break;
					case "DELETE":
						apiController.delete(request, response);
						break;
					}
					response.send();
					foundHandler = true;
					break;
				}
			}

			if (!foundHandler) {
				respond(404, "Not Found", out);
			}
		} catch (IOException e) {
			try {
				e.printStackTrace();
				if (out != null) {
					respond(500, e.toString(), out);
				}
			} catch (IOException e2) {
				e2.printStackTrace();
				// We tried
			}
		} finally {
			try {
				if (out != null) {
					out.close();
				}
				if (in != null) {
					in.close();
				}
				_socket.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}

	private String fixPath(String path) {
		return path;
//			return path.replaceAll("/", "\\/");
	}

}
