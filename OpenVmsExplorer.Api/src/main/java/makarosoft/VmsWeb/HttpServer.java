package makarosoft.VmsWeb;

import java.io.IOException;
import java.util.Map;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import makarosoft.vmsExplorer.Directory.WindowsDirectory;

import java.util.ArrayList;
import java.util.HashMap;
import java.net.ServerSocket;
import java.net.Socket;

public class HttpServer {
	Logger _logger = LogManager.getLogger(HttpServer.class);
	private int _port;
	// Two level map: first level is HTTP Method (GET, POST, OPTION, etc.), second
	// level is the
	// request paths.
	private ArrayList<ApiController> controllers = new ArrayList<ApiController>();

	// TODO SSL support
	public HttpServer(int port) {
		_port = port;
	}

	/**
	 * @param path if this is the special string "/*", this is the default handler
	 *             if no other handler matches.
	 */
	public void addController(String path, ApiController controller) {
		controller.setPath(path);
		controllers.add(controller);
	}

	public void start() throws IOException {
		ServerSocket socket = new ServerSocket(_port);
		_logger.debug("Listening on port {}", _port);
		Socket client;
		while ((client = socket.accept()) != null) {
			_logger.debug("Received connection from {}", client.getRemoteSocketAddress().toString());
			SocketHandler handler = new SocketHandler(client, controllers);
			Thread t = new Thread(handler);
			t.start();
		}
	}

}