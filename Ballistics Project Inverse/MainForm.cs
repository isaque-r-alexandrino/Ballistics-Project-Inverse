using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BallisticCalculator.Models;
using BallisticCalculator.Services;

namespace BallisticCalculator
{
    public partial class MainForm : Form
    {
        // Input controls
        private TextBox txtTargetN, txtTargetE, txtTargetAlt;
        private TextBox txtGunN, txtGunE, txtGunAlt;
        private TextBox txtTemperature, txtPressure, txtHumidity;
        private TextBox txtWindSpeed, txtWindDirection;
        private ComboBox cmbAmmunition;
        private CheckBox chkCoriolis;
        private Button btnCalculate, btnClear;

        // Output controls
        private Label lblAzimuth, lblElevation, lblCharge;
        private Label lblTimeOfFlight, lblRange, lblImpactVelocity;
        private Label lblImpactAngle, lblMaxHeight;
        private Label lblElevCorrection, lblAzimCorrection;
        private Panel resultsPanel;

        private List<Ammunition> ammunitionCatalog;
        private BallisticCalculatorService _calculator;

        public MainForm()
        {
            _calculator = new BallisticCalculatorService();
            InitializeAmmunitionCatalog();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Ballistic Fire Control System";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 9);

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            // Left panel - Input
            Panel inputPanel = CreateInputPanel();
            mainLayout.Controls.Add(inputPanel, 0, 0);

            // Right panel - Results
            resultsPanel = CreateResultsPanel();
            mainLayout.Controls.Add(resultsPanel, 1, 0);

            // Bottom panel - Buttons
            Panel buttonPanel = CreateButtonPanel();
            mainLayout.Controls.Add(buttonPanel, 0, 1);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        private Panel CreateInputPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(248, 248, 248)
            };

            GroupBox grpTarget = new GroupBox
            {
                Text = "Target Data",
                Location = new Point(10, 10),
                Size = new Size(400, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TableLayoutPanel targetLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 9)
            };

            targetLayout.Controls.Add(new Label { Text = "Northing (m):", Anchor = AnchorStyles.Right }, 0, 0);
            txtTargetN = new TextBox { Text = "5000", Width = 120 };
            targetLayout.Controls.Add(txtTargetN, 1, 0);

            targetLayout.Controls.Add(new Label { Text = "Easting (m):", Anchor = AnchorStyles.Right }, 0, 1);
            txtTargetE = new TextBox { Text = "3000", Width = 120 };
            targetLayout.Controls.Add(txtTargetE, 1, 1);

            targetLayout.Controls.Add(new Label { Text = "Altitude (m):", Anchor = AnchorStyles.Right }, 0, 2);
            txtTargetAlt = new TextBox { Text = "100", Width = 120 };
            targetLayout.Controls.Add(txtTargetAlt, 1, 2);

            grpTarget.Controls.Add(targetLayout);

            GroupBox grpGun = new GroupBox
            {
                Text = "Gun Position",
                Location = new Point(10, 140),
                Size = new Size(400, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TableLayoutPanel gunLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 9)
            };

            gunLayout.Controls.Add(new Label { Text = "Northing (m):", Anchor = AnchorStyles.Right }, 0, 0);
            txtGunN = new TextBox { Text = "0", Width = 120 };
            gunLayout.Controls.Add(txtGunN, 1, 0);

            gunLayout.Controls.Add(new Label { Text = "Easting (m):", Anchor = AnchorStyles.Right }, 0, 1);
            txtGunE = new TextBox { Text = "0", Width = 120 };
            gunLayout.Controls.Add(txtGunE, 1, 1);

            gunLayout.Controls.Add(new Label { Text = "Altitude (m):", Anchor = AnchorStyles.Right }, 0, 2);
            txtGunAlt = new TextBox { Text = "50", Width = 120 };
            gunLayout.Controls.Add(txtGunAlt, 1, 2);

            grpGun.Controls.Add(gunLayout);

            GroupBox grpEnv = new GroupBox
            {
                Text = "Environmental Conditions",
                Location = new Point(10, 270),
                Size = new Size(400, 200),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TableLayoutPanel envLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 9)
            };

            envLayout.Controls.Add(new Label { Text = "Temperature (°C):", Anchor = AnchorStyles.Right }, 0, 0);
            txtTemperature = new TextBox { Text = "15", Width = 120 };
            envLayout.Controls.Add(txtTemperature, 1, 0);

            envLayout.Controls.Add(new Label { Text = "Pressure (hPa):", Anchor = AnchorStyles.Right }, 0, 1);
            txtPressure = new TextBox { Text = "1013.25", Width = 120 };
            envLayout.Controls.Add(txtPressure, 1, 1);

            envLayout.Controls.Add(new Label { Text = "Humidity (%):", Anchor = AnchorStyles.Right }, 0, 2);
            txtHumidity = new TextBox { Text = "50", Width = 120 };
            envLayout.Controls.Add(txtHumidity, 1, 2);

            envLayout.Controls.Add(new Label { Text = "Wind Speed (m/s):", Anchor = AnchorStyles.Right }, 0, 3);
            txtWindSpeed = new TextBox { Text = "5", Width = 120 };
            envLayout.Controls.Add(txtWindSpeed, 1, 3);

            envLayout.Controls.Add(new Label { Text = "Wind Direction (°):", Anchor = AnchorStyles.Right }, 0, 4);
            txtWindDirection = new TextBox { Text = "0", Width = 120 };
            envLayout.Controls.Add(txtWindDirection, 1, 4);

            envLayout.Controls.Add(new Label { Text = "Coriolis Effect:", Anchor = AnchorStyles.Right }, 0, 5);
            chkCoriolis = new CheckBox { Text = "Enable", Checked = true };
            envLayout.Controls.Add(chkCoriolis, 1, 5);

            grpEnv.Controls.Add(envLayout);

            GroupBox grpAmmo = new GroupBox
            {
                Text = "Ammunition Selection",
                Location = new Point(10, 480),
                Size = new Size(400, 70),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            cmbAmmunition = new ComboBox
            {
                Location = new Point(10, 25),
                Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbAmmunition.DataSource = ammunitionCatalog;
            cmbAmmunition.DisplayMember = "ToString";
            cmbAmmunition.SelectedIndex = 0;

            grpAmmo.Controls.Add(cmbAmmunition);

            panel.Controls.Add(grpTarget);
            panel.Controls.Add(grpGun);
            panel.Controls.Add(grpEnv);
            panel.Controls.Add(grpAmmo);

            return panel;
        }

        private Panel CreateResultsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(248, 248, 248)
            };

            GroupBox grpResults = new GroupBox
            {
                Text = "Ballistic Solution",
                Location = new Point(10, 10),
                Size = new Size(420, 500),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TableLayoutPanel resultLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 12,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 9)
            };

            // Results rows
            AddResultRow(resultLayout, "Azimuth:", out lblAzimuth, 0);
            AddResultRow(resultLayout, "Elevation:", out lblElevation, 1);
            AddResultRow(resultLayout, "Charge:", out lblCharge, 2);
            AddResultRow(resultLayout, "Time of Flight:", out lblTimeOfFlight, 3);
            AddResultRow(resultLayout, "Range:", out lblRange, 4);
            AddResultRow(resultLayout, "Impact Velocity:", out lblImpactVelocity, 5);
            AddResultRow(resultLayout, "Impact Angle:", out lblImpactAngle, 6);
            AddResultRow(resultLayout, "Max Height:", out lblMaxHeight, 7);
            AddResultRow(resultLayout, "Elevation Correction:", out lblElevCorrection, 8);
            AddResultRow(resultLayout, "Azimuth Correction:", out lblAzimCorrection, 9);

            // Status row
            resultLayout.Controls.Add(new Label { Text = "Status:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 10);
            Label lblStatus = new Label
            {
                Text = "Ready",
                Anchor = AnchorStyles.Left,
                ForeColor = Color.Green,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            resultLayout.Controls.Add(lblStatus, 1, 10);
            resultLayout.SetColumnSpan(lblStatus, 1);

            grpResults.Controls.Add(resultLayout);
            panel.Controls.Add(grpResults);

            return panel;
        }

        private void AddResultRow(TableLayoutPanel layout, string labelText, out Label valueLabel, int row)
        {
            layout.Controls.Add(new Label { Text = labelText, Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 9) }, 0, row);
            valueLabel = new Label
            {
                Text = "--",
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 70, 150)
            };
            layout.Controls.Add(valueLabel, 1, row);
        }

        private Panel CreateButtonPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 60,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            btnCalculate = new Button
            {
                Text = "CALCULATE SOLUTION",
                Location = new Point(10, 5),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCalculate.Click += BtnCalculate_Click;
            panel.Controls.Add(btnCalculate);

            btnClear = new Button
            {
                Text = "CLEAR FIELDS",
                Location = new Point(220, 5),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(200, 200, 200),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClear.Click += (s, e) => ClearFields();
            panel.Controls.Add(btnClear);

            return panel;
        }

        private void InitializeAmmunitionCatalog()
        {
            ammunitionCatalog = new List<Ammunition>
            {
                new Ammunition
                {
                    Id = 1,
                    Name = "M107",
                    ChargeName = "Charge 1",
                    MuzzleVelocity = 684,
                    MaxRange = 14800,
                    ProjectileMass = 43.9,
                    Caliber = 0.155,
                    BallisticCoefficient = 0.7,
                    DragCoefficient = 0.3
                },
                new Ammunition
                {
                    Id = 2,
                    Name = "M795",
                    ChargeName = "Charge 2",
                    MuzzleVelocity = 730,
                    MaxRange = 18500,
                    ProjectileMass = 46.7,
                    Caliber = 0.155,
                    BallisticCoefficient = 0.75,
                    DragCoefficient = 0.28
                },
                new Ammunition
                {
                    Id = 3,
                    Name = "M982 Excalibur",
                    ChargeName = "Charge 3",
                    MuzzleVelocity = 820,
                    MaxRange = 25000,
                    ProjectileMass = 48.0,
                    Caliber = 0.155,
                    BallisticCoefficient = 0.85,
                    DragCoefficient = 0.25
                },
                new Ammunition
                {
                    Id = 4,
                    Name = "M549A1",
                    ChargeName = "Charge 4",
                    MuzzleVelocity = 896,
                    MaxRange = 30000,
                    ProjectileMass = 43.5,
                    Caliber = 0.155,
                    BallisticCoefficient = 0.8,
                    DragCoefficient = 0.26
                },
                new Ammunition
                {
                    Id = 5,
                    Name = "M904",
                    ChargeName = "Charge 5",
                    MuzzleVelocity = 980,
                    MaxRange = 38000,
                    ProjectileMass = 42.8,
                    Caliber = 0.155,
                    BallisticCoefficient = 0.78,
                    DragCoefficient = 0.27
                }
            };
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse inputs
                var target = new TargetData
                {
                    Northing = double.Parse(txtTargetN.Text),
                    Easting = double.Parse(txtTargetE.Text),
                    Altitude = double.Parse(txtTargetAlt.Text)
                };

                var gun = new GunData
                {
                    Northing = double.Parse(txtGunN.Text),
                    Easting = double.Parse(txtGunE.Text),
                    Altitude = double.Parse(txtGunAlt.Text)
                };

                var env = new EnvironmentalData
                {
                    Temperature = double.Parse(txtTemperature.Text),
                    Pressure = double.Parse(txtPressure.Text),
                    Humidity = double.Parse(txtHumidity.Text),
                    WindSpeed = double.Parse(txtWindSpeed.Text),
                    WindDirection = double.Parse(txtWindDirection.Text),
                    CoriolisEffect = chkCoriolis.Checked ? 1.0 : 0.0
                };

                var selectedAmmo = (Ammunition)cmbAmmunition.SelectedItem;

                // Calculate solution
                var solution = _calculator.CalculateSolution(target, gun, env, ammunitionCatalog);

                if (!solution.IsValid)
                {
                    MessageBox.Show("No suitable charge found for the target distance.", "Calculation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Display results
                lblAzimuth.Text = $"{solution.Azimuth:F2} mils";
                lblElevation.Text = $"{solution.Elevation:F2} mils";
                lblCharge.Text = solution.SelectedCharge.ChargeName;
                lblTimeOfFlight.Text = $"{solution.TimeOfFlight:F2} s";
                lblRange.Text = $"{solution.Range:F0} m";
                lblImpactVelocity.Text = $"{solution.ImpactVelocity:F1} m/s";
                lblImpactAngle.Text = $"{solution.ImpactAngle:F1}°";
                lblMaxHeight.Text = $"{solution.MaximumHeight:F0} m";
                lblElevCorrection.Text = $"{solution.ElevationCorrection:F2} mils";
                lblAzimCorrection.Text = $"{solution.AzimuthCorrection:F2} mils";

                resultsPanel.BackColor = Color.FromArgb(220, 240, 220);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values in all fields.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Calculation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtTargetN.Text = "5000";
            txtTargetE.Text = "3000";
            txtTargetAlt.Text = "100";
            txtGunN.Text = "0";
            txtGunE.Text = "0";
            txtGunAlt.Text = "50";
            txtTemperature.Text = "15";
            txtPressure.Text = "1013.25";
            txtHumidity.Text = "50";
            txtWindSpeed.Text = "5";
            txtWindDirection.Text = "0";
            chkCoriolis.Checked = true;
            cmbAmmunition.SelectedIndex = 0;

            lblAzimuth.Text = "--";
            lblElevation.Text = "--";
            lblCharge.Text = "--";
            lblTimeOfFlight.Text = "--";
            lblRange.Text = "--";
            lblImpactVelocity.Text = "--";
            lblImpactAngle.Text = "--";
            lblMaxHeight.Text = "--";
            lblElevCorrection.Text = "--";
            lblAzimCorrection.Text = "--";

            resultsPanel.BackColor = Color.FromArgb(248, 248, 248);
        }
    }
}