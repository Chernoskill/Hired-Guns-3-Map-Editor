
namespace Hired_Guns_3_Map_Editor
{
    partial class ConversationsForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TextBox ConversationsHelpTextBox;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConversationsForm));
            this.EditorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.HorizonsImportButton = new System.Windows.Forms.Button();
            this.CopyEscButton = new System.Windows.Forms.Button();
            this.AddConversationButton = new System.Windows.Forms.Button();
            this.ConversationsPanel = new System.Windows.Forms.Panel();
            this.DeleteConversationButton = new System.Windows.Forms.Button();
            ConversationsHelpTextBox = new System.Windows.Forms.TextBox();
            this.ConversationsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConversationsHelpTextBox
            // 
            ConversationsHelpTextBox.Location = new System.Drawing.Point(86, 378);
            ConversationsHelpTextBox.Multiline = true;
            ConversationsHelpTextBox.Name = "ConversationsHelpTextBox";
            ConversationsHelpTextBox.ReadOnly = true;
            ConversationsHelpTextBox.Size = new System.Drawing.Size(702, 60);
            ConversationsHelpTextBox.TabIndex = 11;
            ConversationsHelpTextBox.Text = resources.GetString("ConversationsHelpTextBox.Text");
            // 
            // HorizonsImportButton
            // 
            this.HorizonsImportButton.Location = new System.Drawing.Point(12, 378);
            this.HorizonsImportButton.Name = "HorizonsImportButton";
            this.HorizonsImportButton.Size = new System.Drawing.Size(68, 25);
            this.HorizonsImportButton.TabIndex = 9;
            this.HorizonsImportButton.Text = "Import";
            this.EditorToolTip.SetToolTip(this.HorizonsImportButton, "Import conversations from a text file. The text file must have the correct format" +
        ": 00 00 00 [Message]");
            this.HorizonsImportButton.UseVisualStyleBackColor = true;
            this.HorizonsImportButton.Click += new System.EventHandler(this.HorizonsImportButton_Click);
            // 
            // CopyEscButton
            // 
            this.CopyEscButton.Location = new System.Drawing.Point(12, 413);
            this.CopyEscButton.Name = "CopyEscButton";
            this.CopyEscButton.Size = new System.Drawing.Size(68, 25);
            this.CopyEscButton.TabIndex = 12;
            this.CopyEscButton.Text = "Copy ESC";
            this.EditorToolTip.SetToolTip(this.CopyEscButton, "Copy the ASCII ESC character to the clipboard so it can be inserted in a conversa" +
        "tion text with CTRL-V.");
            this.CopyEscButton.UseVisualStyleBackColor = true;
            this.CopyEscButton.Click += new System.EventHandler(this.CopyEscButton_Click);
            // 
            // AddConversationButton
            // 
            this.AddConversationButton.Location = new System.Drawing.Point(38, 3);
            this.AddConversationButton.Name = "AddConversationButton";
            this.AddConversationButton.Size = new System.Drawing.Size(29, 23);
            this.AddConversationButton.TabIndex = 3;
            this.AddConversationButton.Text = "+";
            this.AddConversationButton.UseVisualStyleBackColor = true;
            this.AddConversationButton.Click += new System.EventHandler(this.AddConversationButton_Click);
            // 
            // ConversationsPanel
            // 
            this.ConversationsPanel.AutoScroll = true;
            this.ConversationsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ConversationsPanel.Controls.Add(this.DeleteConversationButton);
            this.ConversationsPanel.Controls.Add(this.AddConversationButton);
            this.ConversationsPanel.Location = new System.Drawing.Point(12, 12);
            this.ConversationsPanel.Name = "ConversationsPanel";
            this.ConversationsPanel.Size = new System.Drawing.Size(776, 360);
            this.ConversationsPanel.TabIndex = 10;
            // 
            // DeleteConversationButton
            // 
            this.DeleteConversationButton.Location = new System.Drawing.Point(3, 3);
            this.DeleteConversationButton.Name = "DeleteConversationButton";
            this.DeleteConversationButton.Size = new System.Drawing.Size(29, 23);
            this.DeleteConversationButton.TabIndex = 4;
            this.DeleteConversationButton.Text = "-";
            this.DeleteConversationButton.UseVisualStyleBackColor = true;
            this.DeleteConversationButton.Click += new System.EventHandler(this.DeleteConversationButton_Click);
            // 
            // ConversationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CopyEscButton);
            this.Controls.Add(ConversationsHelpTextBox);
            this.Controls.Add(this.ConversationsPanel);
            this.Controls.Add(this.HorizonsImportButton);
            this.Name = "ConversationsForm";
            this.Text = "Conversations";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConversationsForm_FormClosing);
            this.ConversationsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip EditorToolTip;
        private System.Windows.Forms.Button AddConversationButton;
        private System.Windows.Forms.Button HorizonsImportButton;
        private System.Windows.Forms.Panel ConversationsPanel;
        private System.Windows.Forms.Button CopyEscButton;
        private System.Windows.Forms.Button DeleteConversationButton;
    }
}