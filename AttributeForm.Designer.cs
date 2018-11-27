namespace RxAgent
{
    partial class AttributeForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributeForm));
            this.attributeListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // attributeListBox
            // 
            this.attributeListBox.FormattingEnabled = true;
            resources.ApplyResources(this.attributeListBox, "attributeListBox");
            this.attributeListBox.Name = "attributeListBox";
            this.attributeListBox.SelectedIndexChanged += new System.EventHandler(this.attributeListBox_SelectedIndexChanged);
            // 
            // AttributeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.attributeListBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttributeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AttributeForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox attributeListBox;

    }
}