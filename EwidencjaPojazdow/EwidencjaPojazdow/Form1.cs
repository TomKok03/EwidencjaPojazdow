using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace EwidencjaPojazdow
{
    public class PojazdInfo
    {
        public string NumerRejestracyjny { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public int RokProdukcji { get; set; }
        public DateTime WaznoscPrzegladu { get; set; }
        public string DaneWlasciciela { get; set; }
    }

    public partial class Form1 : Form
    {
        private BindingList<PojazdInfo> _bazaPojazdow;
        private DataGridView dgvLista;
        private TextBox txtRejestracja, txtMarka, txtModel, txtWlasciciel;
        private NumericUpDown numRok;
        private DateTimePicker dtpPrzeglad;
        private PrintDocument _printDoc;

        public Form1()
        {
            InitializeComponent();
            InicjalizacjaDanych();
            BudujInterfejs();

            _printDoc = new PrintDocument();
            _printDoc.PrintPage += PrintDoc_PrintPage;
        }

        private void InicjalizacjaDanych()
        {
            _bazaPojazdow = new BindingList<PojazdInfo>
            {
                new PojazdInfo { NumerRejestracyjny = "WA 12345", Marka = "Ford", Model = "Focus", RokProdukcji = 2018, WaznoscPrzegladu = DateTime.Now.AddDays(5), DaneWlasciciela = "Jan Nowak" },
                new PojazdInfo { NumerRejestracyjny = "KR 55500", Marka = "Opel", Model = "Astra", RokProdukcji = 2020, WaznoscPrzegladu = DateTime.Now.AddMonths(5), DaneWlasciciela = "Firma Budowlana SA" },
                new PojazdInfo { NumerRejestracyjny = "PO 997XY", Marka = "Kia", Model = "Ceed", RokProdukcji = 2022, WaznoscPrzegladu = DateTime.Now, DaneWlasciciela = "Alicja Kowalska" }
            };
        }

        private void BudujInterfejs()
        {
            this.Text = "System Ewidencji Pojazdów";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));

            GroupBox grpLista = new GroupBox { Text = "Lista Pojazdów", Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvLista = new DataGridView
            {
                DataSource = _bazaPojazdow,
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                BackgroundColor = SystemColors.ControlLight,
                BorderStyle = BorderStyle.None
            };
            dgvLista.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvLista.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLista.EnableHeadersVisualStyles = false;

            grpLista.Controls.Add(dgvLista);
            layout.Controls.Add(grpLista, 0, 0);

            Panel panelPrawy = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke, Padding = new Padding(20) };
            FlowLayoutPanel flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false };

            flow.Controls.Add(new Label { Text = "Dane Pojazdu", Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 20) });

            txtRejestracja = DodajPole(flow, "Numer Rejestracyjny");
            txtMarka = DodajPole(flow, "Marka");
            txtModel = DodajPole(flow, "Model");

            flow.Controls.Add(new Label { Text = "Rok produkcji", ForeColor = Color.Gray, Margin = new Padding(0, 10, 0, 0) });
            numRok = new NumericUpDown { Width = 280, Maximum = 2100, Minimum = 1900, Value = DateTime.Now.Year };
            flow.Controls.Add(numRok);

            flow.Controls.Add(new Label { Text = "Data Przeglądu", ForeColor = Color.Gray, Margin = new Padding(0, 10, 0, 0) });
            dtpPrzeglad = new DateTimePicker { Width = 280, Format = DateTimePickerFormat.Short };
            flow.Controls.Add(dtpPrzeglad);

            txtWlasciciel = DodajPole(flow, "Właściciel");

            flow.Controls.Add(new Panel { Height = 25 });

            Button btnDodaj = new Button { Text = "Dodaj Pojazd", BackColor = Color.SeaGreen, ForeColor = Color.White, Height = 45, Width = 280, FlatStyle = FlatStyle.Flat };
            btnDodaj.Click += BtnDodaj_Click;
            flow.Controls.Add(btnDodaj);

            flow.Controls.Add(new Panel { Height = 10 });

            Button btnRaport = new Button { Text = "Drukuj Raport (PDF)", BackColor = Color.RoyalBlue, ForeColor = Color.White, Height = 45, Width = 280, FlatStyle = FlatStyle.Flat };
            btnRaport.Click += BtnRaport_Click;
            flow.Controls.Add(btnRaport);

            panelPrawy.Controls.Add(flow);
            layout.Controls.Add(panelPrawy, 1, 0);

            this.Controls.Add(layout);
        }

        private TextBox DodajPole(FlowLayoutPanel p, string txt)
        {
            p.Controls.Add(new Label { Text = txt, ForeColor = Color.Gray, Margin = new Padding(0, 10, 0, 0) });
            TextBox t = new TextBox { Width = 280, BorderStyle = BorderStyle.FixedSingle };
            p.Controls.Add(t);
            return t;
        }

        private void BtnDodaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRejestracja.Text) || string.IsNullOrWhiteSpace(txtMarka.Text))
            {
                MessageBox.Show("Uzupełnij wymagane pola.");
                return;
            }

            _bazaPojazdow.Add(new PojazdInfo
            {
                NumerRejestracyjny = txtRejestracja.Text.ToUpper(),
                Marka = txtMarka.Text,
                Model = txtModel.Text,
                RokProdukcji = (int)numRok.Value,
                WaznoscPrzegladu = dtpPrzeglad.Value,
                DaneWlasciciela = txtWlasciciel.Text
            });

            txtRejestracja.Clear(); txtMarka.Clear(); txtModel.Clear(); txtWlasciciel.Clear();
        }

        private void BtnRaport_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            pd.Document = _printDoc;
            pd.UseEXDialog = true;

            if (pd.ShowDialog() == DialogResult.OK)
            {
                _printDoc.Print();
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            int m = DateTime.Now.Month;
            int r = DateTime.Now.Year;

            var dane = _bazaPojazdow
                .Where(p => p.WaznoscPrzegladu.Month == m && p.WaznoscPrzegladu.Year == r)
                .ToList();

            Graphics g = e.Graphics;
            Font fNaglowek = new Font("Arial", 16, FontStyle.Bold);
            Font fTekst = new Font("Arial", 10);
            float y = 50;

            g.DrawString($"RAPORT PRZEGLĄDÓW {m:00}/{r}", fNaglowek, Brushes.Black, 50, y);
            y += 40;
            g.DrawLine(Pens.Black, 50, y, 750, y);
            y += 20;

            if (dane.Count == 0)
            {
                g.DrawString("Brak przeglądów w tym miesiącu.", fTekst, Brushes.Black, 50, y);
            }
            else
            {
                g.DrawString("REJESTRACJA   |   POJAZD   |   WŁAŚCICIEL   |   DATA", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, 50, y);
                y += 25;

                foreach (var p in dane)
                {
                    string linia = $"{p.NumerRejestracyjny.PadRight(15)} {p.Marka} {p.Model}   {p.DaneWlasciciela}   {p.WaznoscPrzegladu:dd.MM.yyyy}";
                    g.DrawString(linia, fTekst, Brushes.Black, 50, y);
                    y += 20;
                }
            }
        }
    }
}