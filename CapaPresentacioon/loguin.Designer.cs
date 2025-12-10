namespace CapaPresentacioon
{
    partial class loguin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(loguin));
            pictureBox1 = new PictureBox();
            tbUsuarioLoguin = new MaterialSkin.Controls.MaterialTextBox2();
            tbContrasenaLoguin = new MaterialSkin.Controls.MaterialTextBox2();
            materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            materialButton1 = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(7, 136);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(407, 232);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // tbUsuarioLoguin
            // 
            tbUsuarioLoguin.AnimateReadOnly = false;
            tbUsuarioLoguin.BackgroundImageLayout = ImageLayout.None;
            tbUsuarioLoguin.CharacterCasing = CharacterCasing.Normal;
            tbUsuarioLoguin.Depth = 0;
            tbUsuarioLoguin.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            tbUsuarioLoguin.HideSelection = true;
            tbUsuarioLoguin.LeadingIcon = null;
            tbUsuarioLoguin.Location = new Point(432, 191);
            tbUsuarioLoguin.MaxLength = 32767;
            tbUsuarioLoguin.MouseState = MaterialSkin.MouseState.OUT;
            tbUsuarioLoguin.Name = "tbUsuarioLoguin";
            tbUsuarioLoguin.PasswordChar = '\0';
            tbUsuarioLoguin.PrefixSuffixText = null;
            tbUsuarioLoguin.ReadOnly = false;
            tbUsuarioLoguin.RightToLeft = RightToLeft.No;
            tbUsuarioLoguin.SelectedText = "";
            tbUsuarioLoguin.SelectionLength = 0;
            tbUsuarioLoguin.SelectionStart = 0;
            tbUsuarioLoguin.ShortcutsEnabled = true;
            tbUsuarioLoguin.Size = new Size(254, 48);
            tbUsuarioLoguin.TabIndex = 1;
            tbUsuarioLoguin.TabStop = false;
            tbUsuarioLoguin.TextAlign = HorizontalAlignment.Left;
            tbUsuarioLoguin.TrailingIcon = null;
            tbUsuarioLoguin.UseSystemPasswordChar = false;
            // 
            // tbContrasenaLoguin
            // 
            tbContrasenaLoguin.AnimateReadOnly = false;
            tbContrasenaLoguin.BackgroundImageLayout = ImageLayout.None;
            tbContrasenaLoguin.CharacterCasing = CharacterCasing.Normal;
            tbContrasenaLoguin.Depth = 0;
            tbContrasenaLoguin.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            tbContrasenaLoguin.HideSelection = true;
            tbContrasenaLoguin.LeadingIcon = null;
            tbContrasenaLoguin.Location = new Point(432, 288);
            tbContrasenaLoguin.MaxLength = 32767;
            tbContrasenaLoguin.MouseState = MaterialSkin.MouseState.OUT;
            tbContrasenaLoguin.Name = "tbContrasenaLoguin";
            tbContrasenaLoguin.PasswordChar = '\0';
            tbContrasenaLoguin.PrefixSuffixText = null;
            tbContrasenaLoguin.ReadOnly = false;
            tbContrasenaLoguin.RightToLeft = RightToLeft.No;
            tbContrasenaLoguin.SelectedText = "";
            tbContrasenaLoguin.SelectionLength = 0;
            tbContrasenaLoguin.SelectionStart = 0;
            tbContrasenaLoguin.ShortcutsEnabled = true;
            tbContrasenaLoguin.Size = new Size(254, 48);
            tbContrasenaLoguin.TabIndex = 2;
            tbContrasenaLoguin.TabStop = false;
            tbContrasenaLoguin.TextAlign = HorizontalAlignment.Left;
            tbContrasenaLoguin.TrailingIcon = null;
            tbContrasenaLoguin.UseSystemPasswordChar = false;
            // 
            // materialLabel1
            // 
            materialLabel1.AutoSize = true;
            materialLabel1.Depth = 0;
            materialLabel1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel1.Location = new Point(432, 169);
            materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            materialLabel1.Name = "materialLabel1";
            materialLabel1.Size = new Size(55, 19);
            materialLabel1.TabIndex = 3;
            materialLabel1.Text = "Usuario";
            // 
            // materialLabel2
            // 
            materialLabel2.AutoSize = true;
            materialLabel2.Depth = 0;
            materialLabel2.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel2.Location = new Point(432, 266);
            materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            materialLabel2.Name = "materialLabel2";
            materialLabel2.Size = new Size(82, 19);
            materialLabel2.TabIndex = 4;
            materialLabel2.Text = "Contraseña";
            // 
            // materialButton1
            // 
            materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.Location = new Point(223, 377);
            materialButton1.Margin = new Padding(4, 6, 4, 6);
            materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = Color.Empty;
            materialButton1.Size = new Size(191, 36);
            materialButton1.TabIndex = 5;
            materialButton1.Text = "     Acceder al sistema     ";
            materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = true;
            materialButton1.Click += materialButton1_Click;
            // 
            // loguin
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(821, 521);
            Controls.Add(materialButton1);
            Controls.Add(materialLabel2);
            Controls.Add(materialLabel1);
            Controls.Add(tbContrasenaLoguin);
            Controls.Add(tbUsuarioLoguin);
            Controls.Add(pictureBox1);
            Margin = new Padding(2);
            Name = "loguin";
            Padding = new Padding(4, 80, 4, 4);
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private MaterialSkin.Controls.MaterialTextBox2 tbUsuarioLoguin;
        private MaterialSkin.Controls.MaterialTextBox2 tbContrasenaLoguin;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialButton materialButton1;
    }
}
