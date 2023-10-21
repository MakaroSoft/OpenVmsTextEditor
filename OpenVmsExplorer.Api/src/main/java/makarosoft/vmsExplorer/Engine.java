package makarosoft.vmsExplorer;
import java.io.IOException;

import makarosoft.VmsWeb.HttpServer;
import makarosoft.vmsExplorer.controllers.DirectoryController;
import makarosoft.vmsExplorer.controllers.DiskController;
import makarosoft.vmsExplorer.controllers.FileController;

public class Engine {

	public static void main(String[] args) throws IOException {
		int port = 0;
		
		for (int index = 0; index < args.length; index++) {
			String arg = args[index];
			if (arg.equalsIgnoreCase("-port")) {
				port = Integer.parseInt(args[index+1]);
			}
		}
		
		if (port == 0) {
			System.out.println("port was not set up. Ex: -port 8001");
			return;
		}
		
		HttpServer server = new HttpServer(port);
		server.addController("/api/disk", new DiskController());
		server.addController("/api/directory", new DirectoryController());
		server.addController("/api/File", new FileController());
		server.start();
	}
	
}
