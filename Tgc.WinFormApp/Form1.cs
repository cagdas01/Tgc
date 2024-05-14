using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tgc.Core.Base;
using Tgc.Core.Enums;
using Tgc.Core.Operations.Create;
using Tgc.Core.Operations.Update;

namespace Tgc.WinFormApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = new List<string>() 
            {
                "AccountStructureManagement",
                "BusinessProcessManagement",
                "CardManagement",
                "ConsumerManagement",
                "ContentManagement",
                "DeliveryManagement",
                "IntegrationManagement",
                "InvoiceManagement",
                "MerchantManagement",
                "NotificationManagement",
                "OrderManagement",
                "PaymentManagement",
                "SecurityManagement",
            };

            comboBox2.DataSource = new List<OperationType>()
            {
                OperationType.CreateTrigger,
                OperationType.UpdateTrigger,
                OperationType.Create,
                OperationType.Update,
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var operationType = (OperationType)comboBox2.SelectedValue;
            CqrsBase trigger;
            switch (operationType)
            {
                case OperationType.CreateTrigger:
                    trigger = new CreateEntityTrigger();
                    break;
                case OperationType.UpdateTrigger:
                    trigger = new UpdateEntityTrigger();
                    break;
                case OperationType.Create:
                    trigger = new CreateEntity();
                    break;
                case OperationType.Update:
                    trigger = new UpdateEntity();
                    break;
                default:
                    throw new NotImplementedException($"Operation {operationType} not supported.");
            }

            trigger.moduleName = comboBox1.SelectedItem as string;
            trigger.Process(richTextBox1.Text);

            MessageBox.Show($"İşlem tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            richTextBox1.Text = string.Empty;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
