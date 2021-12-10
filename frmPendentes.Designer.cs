
namespace  CERTEDUC.EnvioLote
{
    partial class frmPendentes
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtFolderID = new System.Windows.Forms.TextBox();
            this.lblDir = new System.Windows.Forms.Label();
            this.btnVerificar = new System.Windows.Forms.Button();
            this.barraProgresso = new System.Windows.Forms.ProgressBar();
            this.gpbResultado = new System.Windows.Forms.GroupBox();
            this.lblNCompleto = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNInt = new System.Windows.Forms.Label();
            this.lblNExt = new System.Windows.Forms.Label();
            this.lblPendExt = new System.Windows.Forms.Label();
            this.lblInt = new System.Windows.Forms.Label();
            this.txtLog1 = new System.Windows.Forms.TextBox();
            this.txtLog2 = new System.Windows.Forms.TextBox();
            this.lblPorCento = new System.Windows.Forms.Label();
            this.btnNotificar = new System.Windows.Forms.Button();
            this.btnGerarLinks = new System.Windows.Forms.Button();
            this.gpbResultado.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFolderID
            // 
            this.txtFolderID.Location = new System.Drawing.Point(85, 29);
            this.txtFolderID.Name = "txtFolderID";
            this.txtFolderID.Size = new System.Drawing.Size(226, 20);
            this.txtFolderID.TabIndex = 1;
            this.txtFolderID.Text = "9667e61c-dd42-4182-f288-08d90bd59fb9";
            // 
            // lblDir
            // 
            this.lblDir.AutoSize = true;
            this.lblDir.Location = new System.Drawing.Point(30, 32);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(53, 13);
            this.lblDir.TabIndex = 3;
            this.lblDir.Text = "Folder ID:";
            // 
            // btnVerificar
            // 
            this.btnVerificar.Location = new System.Drawing.Point(315, 27);
            this.btnVerificar.Name = "btnVerificar";
            this.btnVerificar.Size = new System.Drawing.Size(75, 23);
            this.btnVerificar.TabIndex = 5;
            this.btnVerificar.Text = "Verificar";
            this.btnVerificar.UseVisualStyleBackColor = true;
            this.btnVerificar.Click += new System.EventHandler(this.btnVerificar_Click);
            // 
            // barraProgresso
            // 
            this.barraProgresso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.barraProgresso.Location = new System.Drawing.Point(3, 533);
            this.barraProgresso.Name = "barraProgresso";
            this.barraProgresso.Size = new System.Drawing.Size(817, 38);
            this.barraProgresso.TabIndex = 6;
            // 
            // gpbResultado
            // 
            this.gpbResultado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbResultado.Controls.Add(this.lblNCompleto);
            this.gpbResultado.Controls.Add(this.label2);
            this.gpbResultado.Controls.Add(this.lblNInt);
            this.gpbResultado.Controls.Add(this.lblNExt);
            this.gpbResultado.Controls.Add(this.lblPendExt);
            this.gpbResultado.Controls.Add(this.lblInt);
            this.gpbResultado.Location = new System.Drawing.Point(417, 8);
            this.gpbResultado.Name = "gpbResultado";
            this.gpbResultado.Size = new System.Drawing.Size(488, 61);
            this.gpbResultado.TabIndex = 7;
            this.gpbResultado.TabStop = false;
            this.gpbResultado.Text = "Resultado";
            // 
            // lblNCompleto
            // 
            this.lblNCompleto.Location = new System.Drawing.Point(368, 35);
            this.lblNCompleto.Name = "lblNCompleto";
            this.lblNCompleto.Size = new System.Drawing.Size(74, 13);
            this.lblNCompleto.TabIndex = 5;
            this.lblNCompleto.Text = "0";
            this.lblNCompleto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(368, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Ass. Completo";
            // 
            // lblNInt
            // 
            this.lblNInt.Location = new System.Drawing.Point(42, 35);
            this.lblNInt.Name = "lblNInt";
            this.lblNInt.Size = new System.Drawing.Size(109, 13);
            this.lblNInt.TabIndex = 3;
            this.lblNInt.Text = "0";
            this.lblNInt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNExt
            // 
            this.lblNExt.Location = new System.Drawing.Point(214, 34);
            this.lblNExt.Name = "lblNExt";
            this.lblNExt.Size = new System.Drawing.Size(112, 13);
            this.lblNExt.TabIndex = 2;
            this.lblNExt.Text = "0";
            this.lblNExt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPendExt
            // 
            this.lblPendExt.AutoSize = true;
            this.lblPendExt.Location = new System.Drawing.Point(211, 19);
            this.lblPendExt.Name = "lblPendExt";
            this.lblPendExt.Size = new System.Drawing.Size(115, 13);
            this.lblPendExt.TabIndex = 1;
            this.lblPendExt.Text = "Ass. Pendente Externo";
            // 
            // lblInt
            // 
            this.lblInt.AutoSize = true;
            this.lblInt.Location = new System.Drawing.Point(39, 19);
            this.lblInt.Name = "lblInt";
            this.lblInt.Size = new System.Drawing.Size(112, 13);
            this.lblInt.TabIndex = 0;
            this.lblInt.Text = "Ass. Pendente Interno";
            // 
            // txtLog1
            // 
            this.txtLog1.Location = new System.Drawing.Point(12, 88);
            this.txtLog1.Multiline = true;
            this.txtLog1.Name = "txtLog1";
            this.txtLog1.Size = new System.Drawing.Size(441, 439);
            this.txtLog1.TabIndex = 8;
            // 
            // txtLog2
            // 
            this.txtLog2.Location = new System.Drawing.Point(459, 119);
            this.txtLog2.Multiline = true;
            this.txtLog2.Name = "txtLog2";
            this.txtLog2.Size = new System.Drawing.Size(441, 408);
            this.txtLog2.TabIndex = 9;
            // 
            // lblPorCento
            // 
            this.lblPorCento.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPorCento.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPorCento.Location = new System.Drawing.Point(826, 533);
            this.lblPorCento.Name = "lblPorCento";
            this.lblPorCento.Size = new System.Drawing.Size(74, 38);
            this.lblPorCento.TabIndex = 14;
            this.lblPorCento.Text = "0 %";
            this.lblPorCento.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNotificar
            // 
            this.btnNotificar.Location = new System.Drawing.Point(459, 88);
            this.btnNotificar.Name = "btnNotificar";
            this.btnNotificar.Size = new System.Drawing.Size(441, 25);
            this.btnNotificar.TabIndex = 15;
            this.btnNotificar.Text = "Notificar Pendentes";
            this.btnNotificar.UseVisualStyleBackColor = true;
            this.btnNotificar.Click += new System.EventHandler(this.btnNotificar_Click);
            // 
            // btnGerarLinks
            // 
            this.btnGerarLinks.Location = new System.Drawing.Point(85, 55);
            this.btnGerarLinks.Name = "btnGerarLinks";
            this.btnGerarLinks.Size = new System.Drawing.Size(305, 23);
            this.btnGerarLinks.TabIndex = 16;
            this.btnGerarLinks.Text = "Gerar Links dos Pendentes Externos";
            this.btnGerarLinks.UseVisualStyleBackColor = true;
            this.btnGerarLinks.Click += new System.EventHandler(this.btnGerarLinks_Click);
            // 
            // frmPendentes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 573);
            this.Controls.Add(this.btnGerarLinks);
            this.Controls.Add(this.btnNotificar);
            this.Controls.Add(this.lblPorCento);
            this.Controls.Add(this.txtLog2);
            this.Controls.Add(this.txtLog1);
            this.Controls.Add(this.gpbResultado);
            this.Controls.Add(this.barraProgresso);
            this.Controls.Add(this.btnVerificar);
            this.Controls.Add(this.lblDir);
            this.Controls.Add(this.txtFolderID);
            this.Name = "frmPendentes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assinaturas Pendentes";
            this.Load += new System.EventHandler(this.frmPendentes_Load);
            this.gpbResultado.ResumeLayout(false);
            this.gpbResultado.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtFolderID;
        private System.Windows.Forms.Label lblDir;
        private System.Windows.Forms.Button btnVerificar;
        private System.Windows.Forms.ProgressBar barraProgresso;
        private System.Windows.Forms.GroupBox gpbResultado;
        private System.Windows.Forms.TextBox txtLog1;
        private System.Windows.Forms.TextBox txtLog2;
        private System.Windows.Forms.Label lblNInt;
        private System.Windows.Forms.Label lblNExt;
        private System.Windows.Forms.Label lblPendExt;
        private System.Windows.Forms.Label lblInt;
        private System.Windows.Forms.Label lblPorCento;
        private System.Windows.Forms.Label lblNCompleto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnNotificar;
        private System.Windows.Forms.Button btnGerarLinks;
    }
}