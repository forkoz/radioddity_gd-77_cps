using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;

namespace DMR
{
	public partial class DMRIDForm : Form
	{
		public static byte[] DMRIDBuffer = new byte[0x40000];

		static  List<DMRDataItem> DataList = null;
		private static byte[] SIG_PATTERN_BYTES;
		private WebClient _wc;
		private bool _isDownloading = false;
		//private int MAX_RECORDS = 10920;
		private int _stringLength = 8;
		const int HEADER_LENGTH = 12;

		public static void ClearStaticData()
		{
			DMRIDBuffer = new byte[0x40000];
		}

		public DMRIDForm()
		{
			SIG_PATTERN_BYTES = new byte[] { 0x49, 0x44, 0x2D, 0x56, 0x30, 0x30, 0x31, 0x00 };
			InitializeComponent();
			groupBox1.Visible = false; // This is redundant. So hide it until I have time to remove it.
			cmbStringLen.Visible = false;
			lblEnhancedLength.Visible = false;

			txtRegionId.Text = (int.Parse(GeneralSetForm.data.RadioId) / 10000).ToString();

			DataList = new List<DMRDataItem>();

			cmbStringLen.SelectedIndex = 2;


			dataGridView1.AutoGenerateColumns = false;
			DataGridViewCell cell = new DataGridViewTextBoxCell();
			DataGridViewTextBoxColumn colFileName = new DataGridViewTextBoxColumn()
			{
				CellTemplate = cell,
				Name = "Id", // internal name
				HeaderText = "ID",// Column header text
				DataPropertyName = "DMRID" // object property
			};
			dataGridView1.Columns.Add(colFileName);

			cell = new DataGridViewTextBoxCell();
			colFileName = new DataGridViewTextBoxColumn()
			{
				CellTemplate = cell,
				Name = "Call",// internal name
				HeaderText = "Callsign",// Column header text
				DataPropertyName = "Callsign"  // object property
			};
			dataGridView1.Columns.Add(colFileName);

			cell = new DataGridViewTextBoxCell();
			colFileName = new DataGridViewTextBoxColumn()
			{
				CellTemplate = cell,
				Name = "Name",// internal name
				HeaderText = "Name",// Column header text
				DataPropertyName = "Name"  // object property
			};
			dataGridView1.Columns.Add(colFileName);


			cell = new DataGridViewTextBoxCell();
			colFileName = new DataGridViewTextBoxColumn()
			{
				CellTemplate = cell,
				Name = "Age",// internal name
				HeaderText = "Last heard (days ago)",// Column header text
				DataPropertyName = "AgeInDays",  // object property
				Width = 140,
				ValueType = typeof(int)
			};
			dataGridView1.Columns.Add(colFileName);
			dataGridView1.UserDeletedRow += new DataGridViewRowEventHandler(dataGridRowDeleted);

			rebindData();	
		}

		private void dataGridRowDeleted(object sender, DataGridViewRowEventArgs e)
		{
			updateTotalNumberMessage();
		}

		private void rebindData()
		{
			var bindingList = new BindingList<DMRDataItem>(DataList);
			var source = new BindingSource(bindingList, null);
			dataGridView1.DataSource = source;
			updateTotalNumberMessage();
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
			if (DataList == null || _isDownloading)
			{
				return;
			}

			_wc = new WebClient();
			try
			{
				lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"];
				Cursor.Current = Cursors.WaitCursor;
				this.Refresh();
				Application.DoEvents();
				_wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DMRMARCDownloadCompleteHandler);
				_wc.DownloadStringAsync(new Uri("http://ham-digital.org/user_by_lh.php?id=" + txtRegionId.Text));
	
			}
			catch (Exception )
			{
				Cursor.Current = Cursors.Default;
				MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
				return;
			}
			_isDownloading = true;

		}


		private void DMRMARCDownloadCompleteHandler(object sender, DownloadStringCompletedEventArgs e )
		{
			string ownRadioId = GeneralSetForm.data.RadioId;
			string csv;// = e.Result;
			int maxAge = Int32.MaxValue;


			try
			{
				csv = e.Result;
			}
			catch(Exception)
			{
				MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
				return;
			}

			try
			{
				maxAge = Int32.Parse(this.txtAgeMaxDays.Text);
			}
			catch(Exception)
			{

			}

			try
			{
				bool first = true;
				foreach (var csvLine in csv.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (first)
					{
						first = false;
						continue;
					}
					DMRDataItem item = (new DMRDataItem()).FromHamDigital(csvLine);
					if (item.AgeAsInt <= maxAge)
					{
						DataList.Add(item);
					}
				}
				DataList = DataList.Distinct().ToList();

				rebindData();
				//DataToCodeplug();
				Cursor.Current = Cursors.Default;
	
			}
			catch (Exception ex)
			{
				MessageBox.Show(Settings.dicCommon["ErrorParsingData"]);
			}
			finally
			{
				_wc = null;
				_isDownloading = false;
				Cursor.Current = Cursors.Default;
			}
		}

		private void updateTotalNumberMessage()
		{
			string message = Settings.dicCommon["DMRIdContcatsTotal"];// "Total number of IDs = {0}. Max of MAX_RECORDS can be uploaded";
			lblMessage.Text = string.Format(message, DataList.Count, MAX_RECORDS);
		}

		private void downloadProgressHandler(object sender, DownloadProgressChangedEventArgs e)
		{
			try
			{
				BeginInvoke((Action)(() =>
				{
					lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"] + e.ProgressPercentage + "%";
				}));
			}
			catch (Exception)
			{
				// No nothing
			}
		}

		private void btnReadFromGD77_Click(object sender, EventArgs e)
		{

			MainForm.CommsBuffer = new byte[0x100000];// 128k buffer
			CodeplugComms.CommunicationMode = CodeplugComms.CommunicationType.dataRead;
			CommPrgForm commPrgForm = new CommPrgForm(true);// true =  close download form as soon as download is complete
			commPrgForm.StartPosition = FormStartPosition.CenterParent;
			CodeplugComms.startAddress = 0x50100;
			CodeplugComms.transferLength = 0x20;
			DialogResult result = commPrgForm.ShowDialog();
			Array.Copy(MainForm.CommsBuffer, 0x50100, DMRIDForm.DMRIDBuffer, 0, 0x20);
			if (!isInMemoryAccessMode(DMRIDForm.DMRIDBuffer))
			{
				MessageBox.Show(Settings.dicCommon["EnableMemoryAccessMode"]);
				return;
			}

			CodeplugComms.startAddress = 0x30000;
			CodeplugComms.transferLength = 0x20;
			result = commPrgForm.ShowDialog();
			Array.Copy(MainForm.CommsBuffer, 0x30000, DMRIDForm.DMRIDBuffer, 0, 0x20);


			int numRecords = BitConverter.ToInt32(DMRIDForm.DMRIDBuffer, 8);
			int stringLen = (int)DMRIDForm.DMRIDBuffer[3]-0x4a - 4;
			if (stringLen!=8)
			{
				chkEnhancedFirmware.Checked=true;
			}
			cmbStringLen.SelectedIndex = stringLen - 6;
			
			CodeplugComms.startAddress = 0x30000;
			CodeplugComms.transferLength = Math.Min((this.chkEnhancedFirmware.Checked == true ? 0x40000 : 0x20000), HEADER_LENGTH + (numRecords + 2) * (4 + _stringLength));

			CodeplugComms.CommunicationMode = CodeplugComms.CommunicationType.dataRead;
			result = commPrgForm.ShowDialog();
			Array.Copy(MainForm.CommsBuffer, 0x30000, DMRIDForm.DMRIDBuffer, 0, CodeplugComms.transferLength);
			radioToData();
			rebindData();
			//DataToCodeplug();
		}

		private void radioToData()
		{
			byte[] buf = new byte[(4 + _stringLength)];
			DataList = new List<DMRDataItem>();
			int numRecords = BitConverter.ToInt32(DMRIDForm.DMRIDBuffer, 8);
			for (int i = 0; i < numRecords; i++)
			{
				Array.Copy(DMRIDForm.DMRIDBuffer, HEADER_LENGTH + i * (4 + _stringLength), buf, 0, (4 + _stringLength));
				DataList.Add((new DMRDataItem()).FromRadio(buf, _stringLength));
			}
		}

		public void CodeplugToData()
		{
			byte[] buf = new byte[(4 + _stringLength)];
			DataList = new List<DMRDataItem>();
			int numRecords = BitConverter.ToInt32(DMRIDForm.DMRIDBuffer, 8);// Number of records is stored at offset 8
			for (int i = 0; i < numRecords; i++)
			{
				Array.Copy(DMRIDForm.DMRIDBuffer, HEADER_LENGTH + i * (4 + _stringLength), buf, 0, (4 + _stringLength));
				DataList.Add(new DMRDataItem(buf, _stringLength));
			}
		}

		private int MAX_RECORDS
		{
			get
			{
				return ((this.chkEnhancedFirmware.Checked==true?0x40000:0x20000) - HEADER_LENGTH) / (_stringLength + 4);
			}
		}

		private byte[] GenerateUploadData()
		{

			int numRecords = Math.Min(DataList.Count, MAX_RECORDS);
			int dataSize = numRecords * (4 + _stringLength) + HEADER_LENGTH;
			dataSize = ((dataSize / 32)+1) * 32;
			byte[] buffer = new byte[dataSize];

			Array.Copy(SIG_PATTERN_BYTES, buffer, SIG_PATTERN_BYTES.Length);
			Array.Copy(BitConverter.GetBytes(numRecords), 0, buffer, 8, 4);

			if (DataList == null)
			{
				return buffer;
			}
			List<DMRDataItem> uploadList = new List<DMRDataItem>(DataList);// Need to make a copy so we can sort it and not screw up the list in the dataGridView
			uploadList.Sort();
			for (int i = 0; i < numRecords; i++)
			{
				Array.Copy(uploadList[i].getRadioData(rbtnName.Checked,_stringLength), 0, buffer, HEADER_LENGTH + i * (4 + _stringLength), (4 + _stringLength));
			}
			return buffer;
		}

		private bool isInMemoryAccessMode(byte []buffer)
		{
			for (int i = 0; i < 0x20; i++)
			{
				if (buffer[i] != 00)
				{
					return true;
				}
			}
			return false;
		}

		private bool hasSig()
		{
			
			for (int i = 0; i < SIG_PATTERN_BYTES.Length; i++)
			{
				if (DMRIDForm.DMRIDBuffer[i] != SIG_PATTERN_BYTES[i])
				{
				return false;
				}
			}
			return true;
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			DataList = new List<DMRDataItem>();
			rebindData();
			//DataToCodeplug();

		}

		private void btnWriteToGD77_Click(object sender, EventArgs e)
		{
			MainForm.CommsBuffer = new byte[0x100000];// 128k buffer
			CodeplugComms.CommunicationMode = CodeplugComms.CommunicationType.dataRead;
			CommPrgForm commPrgForm = new CommPrgForm(true);// true =  close download form as soon as download is complete
			commPrgForm.StartPosition = FormStartPosition.CenterParent;
			CodeplugComms.startAddress = 0x50100;
			CodeplugComms.transferLength = 0x20;
			DialogResult result = commPrgForm.ShowDialog();
			Array.Copy(MainForm.CommsBuffer, 0x50100, DMRIDForm.DMRIDBuffer, 0, 0x20);
			if (!isInMemoryAccessMode(DMRIDForm.DMRIDBuffer))
			{
				MessageBox.Show(Settings.dicCommon["EnableMemoryAccessMode"]); 
				return;
			}



			SIG_PATTERN_BYTES[3] = (byte)(0x4a + _stringLength + 4);

			byte []uploadData = GenerateUploadData();
			Array.Copy(uploadData, 0, MainForm.CommsBuffer, 0x30000, (uploadData.Length/32)*32);
			CodeplugComms.CommunicationMode = CodeplugComms.CommunicationType.dataWrite;

			CodeplugComms.startAddress = 0x30000;
			CodeplugComms.transferLength = (uploadData.Length/32)*32;
			commPrgForm.StartPosition = FormStartPosition.CenterParent;
			result = commPrgForm.ShowDialog();
		}

		private void DMRIDFormNew_FormClosing(object sender, FormClosingEventArgs e)
		{
			//DataToCodeplug();
		}

		private void DMRIDForm_Load(object sender, EventArgs e)
		{
			Settings.smethod_59(base.Controls);
			Settings.smethod_68(this);// Update texts etc from language xml file
		}

		private void chkEnhancedFirmware_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chkEnhancedFirmware.Checked == false)
			{
				cmbStringLen.Visible = false;
				lblEnhancedLength.Visible = false;
				cmbStringLen.SelectedIndex = 2;
			}
			else
			{

				if (DialogResult.Yes == MessageBox.Show("This mode ONLY works with the community firmware installed in the GD-77.\n\nDo not use this mode if you are using the official firmware\nYou use it at your own risk.\n\nUploading the DMR ID's to the Radioddity GD-77, using this feature, could potentially damage your radio.\n\nBy clicking 'Yes' you acknowledge that you use this feature entirely at your own risk", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2))
				{
					cmbStringLen.Visible = true;
					lblEnhancedLength.Visible = true;
				}
				else
				{
					this.Close();
				}
			}
			
			updateTotalNumberMessage();
		}

		private void cmbStringLen_SelectedIndexChanged(object sender, EventArgs e)
		{
			_stringLength = cmbStringLen.SelectedIndex + 6;
			updateTotalNumberMessage();
		}
	}
}

