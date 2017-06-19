using System;
using System.Windows.Forms;

namespace VHCom_users {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			bool result;
			var mutex = new System.Threading.Mutex(true, "VHComUserSettings", out result);

			if(!result) {
				MessageBox.Show("Csak az egypéldányos futás engedélyezett.","Figyelmtetés",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new vhcomUserSettings());
			GC.KeepAlive(mutex);
		}
	}
}
