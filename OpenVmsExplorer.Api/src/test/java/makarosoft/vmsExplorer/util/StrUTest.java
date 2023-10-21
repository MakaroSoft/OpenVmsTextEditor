package makarosoft.vmsExplorer.util;

import static org.junit.Assert.*;

import org.junit.Test;

public class StrUTest {
	
	@Test
	public void containsIgnoreCase_DIR_returns_true() {
		boolean actual = StrU.containsIgnoreCase("myfolder.DIR;1", ".DIR;");
		assertEquals(true, actual);
	}	
	
	@Test
	public void containsIgnoreCase_dir_returns_true() {
		boolean actual = StrU.containsIgnoreCase("myfolder.dir;1", ".DIR;");
		assertEquals(true, actual);
	}	
	
	@Test
	public void containsIgnoreCase_DiR_returns_true() {
		boolean actual = StrU.containsIgnoreCase("myfolder.DiR;1", ".DIR;");
		assertEquals(true, actual);
	}	
}
