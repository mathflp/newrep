using CERTEDUC.EnvioLote.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  CERTEDUC.EnvioLote
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new frmGestao());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();


            if (!args.Name.Split(',')[0].ToString().Contains("resources"))
            {
                byte[] objReference = (byte[])Resources.ResourceManager.GetObject(args.Name.Split(',')[0].ToString().Replace(".", "_"));

                if (objReference != null)
                {
                    return Assembly.Load(objReference);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
