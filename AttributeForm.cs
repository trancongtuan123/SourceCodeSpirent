using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ranorex;
using Ranorex.Core;

namespace RxAgent
{
    public partial class AttributeForm : System.Windows.Forms.Form
    {

        public ElementInfo currentElement;

        public delegate void SelectAttribute(string name, string value);

        public event SelectAttribute AttributeSelected;

        private bool listInitialized;

        public AttributeForm()
        {
            InitializeComponent();
        }

        public void FillAtributes(ElementInfo element, IList<AttributeDescriptor> attributes)
        {
            currentElement = element;
            List<string> attributesSource = new List<string>();

            foreach (AttributeDescriptor attribute in attributes)
            {
                string value = element.Element.GetAttributeValueText(attribute.Name);
                attributesSource.Add(attribute.DisplayName + "=" + value);

            }
            listInitialized = false;
            attributeListBox.DataSource = attributesSource;
            attributeListBox.AutoSize = true;
            listInitialized = true;
        }

        private void attributeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listInitialized)
            {
                return;
            }
            string value = attributeListBox.SelectedValue.ToString();
            string[] split = value.Split(new char[]{'='});
            AttributeSelected(split[0], split[1]);
            this.Hide();
        }

        private void AttributeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

    }
}
