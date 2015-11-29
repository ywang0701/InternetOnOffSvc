namespace InternetOnOffSvc
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InternetOnOff = new System.ServiceProcess.ServiceProcessInstaller();
            this.InternetOnOffSvc = new System.ServiceProcess.ServiceInstaller();
            // 
            // InternetOnOff
            // 
            this.InternetOnOff.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.InternetOnOff.Password = null;
            this.InternetOnOff.Username = null;
            // 
            // InternetOnOffSvc
            // 
            this.InternetOnOffSvc.DelayedAutoStart = true;
            this.InternetOnOffSvc.DisplayName = "InternetOnOffSvc";
            this.InternetOnOffSvc.ServiceName = "InternetOnOffSvc";
            this.InternetOnOffSvc.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.InternetOnOff,
            this.InternetOnOffSvc});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller InternetOnOff;
        private System.ServiceProcess.ServiceInstaller InternetOnOffSvc;
    }
}