package makarosoft.VmsWeb;

import java.io.IOException;
import java.util.Map;
import java.util.ArrayList;
import java.util.HashMap;
import java.net.ServerSocket;
import java.net.Socket;

public class HttpServer {
	private int port;
	// Two level map: first level is HTTP Method (GET, POST, OPTION, etc.), second
	// level is the
	// request paths.
	private ArrayList<ApiController> controllers = new ArrayList<ApiController>();

	// TODO SSL support
	public HttpServer(int port) {
		this.port = port;
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
		ServerSocket socket = new ServerSocket(port);
		System.out.println("Listening on port " + port);
		Socket client;
		while ((client = socket.accept()) != null) {
			System.out.println("Received connection from " + client.getRemoteSocketAddress().toString());
			SocketHandler handler = new SocketHandler(client, controllers);
			Thread t = new Thread(handler);
			t.start();
		}
	}

}