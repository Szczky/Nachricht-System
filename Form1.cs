public partial class Form1 : Form
    {
        public const string SERVER = "SERVER = localhost; UID = root; PASSWORD = ; DATABASE = secudrive";   //zuhause_ passwd=
        private string status = "";
        private int id;
        private int alteId = -1;
        private List<CheckBox> chbList = new List<CheckBox>();
        List<Image> bildList = new List<Image>();
        int zaehler = 0;
        private bool istAngekommen;
        private bool meineAufgabe = true;
        private Nachricht aktuelNachricht = new Nachricht();
        private List<Mitarbeiter> ausgewaehlteMitList = new List<Mitarbeiter>();
        private int geixtAnzahl = 0;
        int aufgabeZeit = 0;
        int aufgabePrior = 0;
        int aufgabeKate = 0;
        int aufgabeStat = 0;
        DateTime[] aufgabeDaten;
        private List<int> kontaktList = new List<int>();
        public Form1()
        {
            InitializeComponent();
            istAngekommen = true;
            lSecudrive.Location = new System.Drawing.Point(this.Width / 2 - lSecudrive.Size.Width / 2, 9);
            chbList.Clear();
            panelLog.Visible = true;
            panelLog.Location = new System.Drawing.Point(this.Width / 2 - panelLog.Size.Width / 2, this.Height / 2 - panelLog.Size.Height);
            panelRahme.Location = new System.Drawing.Point(this.Width / 2 - panelRahme.Size.Width / 2, this.Height/2-panelRahme.Size.Height/2+15);
            panelNachricht.Parent = panelRahme;
            panelNachricht.Location = new System.Drawing.Point(0, 70);
            panelLogEinstell.Parent = panelRahme;
            panelLogEinstell.Location = new System.Drawing.Point(0, 70);

         //--------
            tbUser.Text = "";
            tbPwd.Text = "";
            bLogok.Focus();
         //--------
            panelRahme.Visible = false;
            panelNachricht.Visible = false;
            lHello.Visible = false;
            Mitarbeiter.mitarbeiterLaden();
            Person.personLaden();
            for (int i = 0; i < 11; i++)
            {
                Image img = Image.FromFile("Bild\\Button\\" + i + ".gif");
                bildList.Add(img);
            }
            bNachrichten.BackgroundImage = bildList[0];
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            lSecudrive.Location = new System.Drawing.Point(this.Width / 2 - lSecudrive.Size.Width / 2, 9);
            panelLog.Location = new System.Drawing.Point(this.Width / 2 - panelLog.Size.Width / 2, this.Height / 2 - panelLog.Size.Height);
            lHello.Location = new System.Drawing.Point(this.Width / 2 - lHello.Size.Width / 2, +lSecudrive.Location.Y + lSecudrive.Size.Height + 5);
            panelRahme.Location = new System.Drawing.Point(this.Width / 2 - panelRahme.Size.Width / 2, 60);
        }        
		
		private void bLogok_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(SERVER);
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select person_id, vname, nname, mitarbeiter.stat from mitarbeiter join person on mitarbeiter.person_id = person.id where log=@log and pw=md5(@pw) and mitarbeiter.aktiv=true;";
                cmd.Parameters.AddWithValue("@log", tbUser.Text);
                cmd.Parameters.AddWithValue("@pw", tbPwd.Text);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    id = reader.GetInt32("person_id");
                    string vname = reader.GetString("vname");
                    string nname = reader.GetString("nname");
                    status = reader.GetString("stat");

                    unvisible();
                    panelRahme.Visible = true;
                    panelNachricht.Visible = true;
                    lHello.Visible = true;
                    lHello.Text = "Hertzliche Willkommen " + vname + " " + nname;
                    Nachricht.aNachrichtListLaden(Nachricht.aNachList, id);
                    Nachricht.sNachrichtListLaden(id);
                    Aufgabe.aufgabeZuMitLaden(id);
                    dgvNLaden(Nachricht.aNachList);
                    istAngekommen = true;
                    timer.Start();
                    if (status.Equals("admin"))
                    {
                        bMitVerw.Visible = true;
                        Aufgabe.aufgabeAlleLaden();
                    }
                    lHello.Visible = true;
                    lHello.Location = new System.Drawing.Point(this.Width / 2 - lHello.Size.Width / 2, 56);
                }
                else
                {
                    MessageBox.Show("False Anmeldungeingabe!", "Hoopla!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbUser.Text = "";
                    tbPwd.Text = "";
                    tbUser.Focus();
                }
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bLogEinstellung_Click(object sender, EventArgs e) 
        {
            unvisible();
            panelLogEinstell.Visible = true;
            panelNeuLog.Visible = false;
            tbAlteUser.Text = "";
            tbAltePw.Text = "";
            tbAlteUser.Focus();
        }
        private void bAlteOk_Click(object sender, EventArgs e)
        {
            alteId = -1;
            try
            {
                MySqlConnection conn = new MySqlConnection(SERVER);
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select person_id from mitarbeiter where log=@log and pw=md5(@pw);";
                cmd.Parameters.AddWithValue("@log", tbAlteUser.Text);
                cmd.Parameters.AddWithValue("@pw", tbAltePw.Text);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    alteId = reader.GetInt32("person_id");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (alteId < 0)
            {
                MessageBox.Show("False Anmeldungdaten!", "Hoopla", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbAltePw.Text = "";
                tbAltePw.Focus();
                return;
            }
            panelNeuLog.Visible = true;
            tbNeuUser.Text = "";
            tbNeuPw.Text = "";
            tbNuePw2.Text = "";
            tbNeuUser.Focus();
        }

        private void bNueLog_Click(object sender, EventArgs e)
        {
            if (tbNeuPw.Text.Equals(tbNuePw2.Text))
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(Form1.SERVER);
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "Update mitarbeiter set log=@log, pw=md5(@pw) where person_id=@id; "; 
                    cmd.Parameters.AddWithValue("id", alteId);
                    cmd.Parameters.AddWithValue("log", tbNeuUser.Text);
                    cmd.Parameters.AddWithValue("pw", tbNeuPw.Text);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Username- und Passwordänderung sind gelungen!", "Veränderte Login Daten", MessageBoxButtons.OK);
                    eingehend();
                }
                
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062) // Cannot insert duplicate key row in object error
                    {
                        MessageBox.Show(" Fehler\nDiese Username existiert schon!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbNeuUser.Text = "";
                        tbNeuUser.Focus();
                    } 
                    else MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Die Zweie Kennwörter sind nicht gleich!!", "Verschiedenen Passwörter!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbNuePw2.Text = "";
                tbNeuPw.Text = "";
                tbNeuPw.Focus();
            }
        }

        private void bLogAus_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Möchten Sie sich abmelden?", "Abmelden", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                id = 0;
                status = "";
                unvisible();
                panelRahme.Visible = false;
                lHello.Visible = false;
                panelLog.Visible = true;
                tbUser.Text = "";
                tbPwd.Text = "";
                tbUser.Focus();
                timer.Stop();
            }
        }
        private void dgvNLaden(List<Nachricht> nachList)
        {
            geixtAnzahl = 0;
            dgvNachricht.Rows.Clear();
            if (istAngekommen)
            {
                int i = 0;
                dgvNachricht.Columns[2].HeaderText = "Sender";
                foreach (Nachricht n in nachList)
                {
                    dgvNachricht.Rows.Add(n.Id, false, n.Sender_Empfanger[0].Nname + ", " + n.Sender_Empfanger[0].Vname, n.Sub, n.Datum.ToString("dd.MM.yyyy hh:mm"));   //ennek kell elöl állnia
                    if (n.Geguckt)                                                                                                          //amig nem létezik a sor, 
                    {                                                                                                                       //nem lehet beállítani
                        dgvNachricht.Rows[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                    }
                    else
                    {
                        dgvNachricht.Rows[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                    }
                    i++;
                }
            }
            else
            {
                dgvNachricht.RowsDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                dgvNachricht.Columns[2].HeaderText = "Empfänger";
                foreach (Nachricht n in nachList)
                {
                    string empfaenger = "";
                    foreach (Mitarbeiter m in n.Sender_Empfanger)
                    {
                        empfaenger += m.Nname + ", " + m.Vname + "  *  ";
                    }
                    dgvNachricht.Rows.Add(n.Id, false, empfaenger, n.Sub, n.Datum);
                }
            }
        }
        private void eingehend()
        {
            panelLesen.Visible = false;
            panelAusgew.Visible = true;
            panelAntwort.Visible = false;
            panelNeuNachricht.Visible = false;
            bNachrichten.BackgroundImage = bildList[0];
            istAngekommen = true;
            Nachricht.aNachrichtListLaden(Nachricht.aNachList, id);
            dgvNLaden(Nachricht.aNachList);
            geixtAnzahl = 0;
            bAusgewLoeschen.Visible = false;
            bUngelesen.Visible = false;
            bZurueckNachr.Visible = false;
            bGelesen.Visible = false;
            panelNachricht.Visible = true;
            bAntwort.Visible = false;
        }

        private void bNeuNachricht_Click(object sender, EventArgs e)
        {
            lEmpf.Text = "";
            panelAntwort.Visible = false;
            bAntwort.Visible = false;
            panelNeuNachricht.Visible = true;
            ausgewaehlteMitList.Clear();
            cbMitarbeiter.Items.Clear();
            lbAusgew.Items.Clear();
            tbSub.Text = "";
            rtbNachricht.Text = "";
            lSub.Text = "Betreff: ";
            tbSub.Focus();
        }

        private void bEingehend_Click(object sender, EventArgs e)
        {
            eingehend();
        }

        private void bAusgehend_Click(object sender, EventArgs e)
        {
            panelAntwort.Visible = false;
            panelAusgew.Visible = true;
            panelNeuNachricht.Visible = false;
            panelLesen.Visible = false;
            istAngekommen = false;
            bAntwort.Visible = false;
            dgvNLaden(Nachricht.sNachList);
            geixtAnzahl = 0;
            bAusgewLoeschen.Visible = false;
            bUngelesen.Visible = false;
            bGelesen.Visible = false;
            bZurueckNachr.Visible = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            zaehler++;
            if (zaehler % 50 == 0)
            {
                int anzahl = Nachricht.aNachList.Count();
                int neuAnzahl = anzahl;
                try
                {
                    MySqlConnection conn = new MySqlConnection(SERVER);
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select count(nachricht_id) as neuAnzahl from nachricht_empfanger WHERE mitarbeiter_id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Prepare();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        neuAnzahl = reader.GetInt32("neuAnzahl");
                    }
                    reader.Close();
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString() + "Fehler beim Aktualisieren der Nachrichten. Bitte überprüfen Sie Ihre Verbindung zum Server!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (anzahl < neuAnzahl)
                {
                    switch (neuAnzahl - anzahl)
                    {
                        case 1:
                            bNachrichten.BackgroundImage = bildList[1];
                            break;
                        case 2:
                            bNachrichten.BackgroundImage = bildList[2];
                            break;
                        case 3:
                            bNachrichten.BackgroundImage = bildList[3];
                            break;
                        case 4:
                            bNachrichten.BackgroundImage = bildList[4];
                            break;
                        case 5:
                            bNachrichten.BackgroundImage = bildList[5];
                            break;
                        case 6:
                            bNachrichten.BackgroundImage = bildList[6];
                            break;
                        case 7:
                            bNachrichten.BackgroundImage = bildList[7];
                            break;
                        case 8:
                            bNachrichten.BackgroundImage = bildList[8];
                            break;
                        case 9:
                            bNachrichten.BackgroundImage = bildList[9];
                            break;
                        default:
                            bNachrichten.BackgroundImage = bildList[10];
                            break;
                    }
                }
                else bNachrichten.BackgroundImage = bildList[0];
            }
        }
       
        private void dgvNachricht_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            panelAusgew.Visible = false;
            panelLesen.Visible = true;
            rtbAntwort.Text = "";
            bZurueckNachr.Visible = true;
            bAntwort.Visible = true;
            int index = (int)dgvNachricht.SelectedCells[0].Value;
            if (istAngekommen)
            {
                int i = -1;
                Nachricht n = Nachricht.aNachList.Find(
                        delegate (Nachricht nach)
                        {
                            i++;
                            return nach.Id == index;
                        }
                    );
                lSend.Text = n.Sender_Empfanger[0].Nname + ", " + n.Sender_Empfanger[0].Vname;
                n.gelesen();
                Nachricht.aNachList[i].Geguckt = true;
                lBetreff.Text = n.Sub;
                rtbNachText.Text = n.textNachtricht();
                aktuelNachricht = n;
                panelAntwort.Visible = true;
                dgvNLaden(Nachricht.aNachList);
            }
            else
            {
                Nachricht n = Nachricht.sNachList.Find(
                        delegate (Nachricht nach)
                        {
                            return nach.Id == index;
                        }
                    );
                lSend.Text = "";
                lBetreff.Text = n.Sub;
                foreach (Mitarbeiter m in n.Sender_Empfanger)
                {
                    lSend.Text += m.Nname + ", " + m.Vname + "  *  ";
                }
                rtbNachText.Text = n.textNachtricht();
            }
        }
        private void bNachrichten_Click(object sender, EventArgs e)
        {
            unvisible();
            eingehend();
        }      

        private void bZurueckNachr_Click(object sender, EventArgs e)
        {
            panelLesen.Visible = false;
            bLoeschenAusw.Visible = false;
            bUngelesen.Visible = false;
            bZurueckNachr.Visible = false;
            geixtAnzahl = 0;
            bAntwort.Visible = false;
            panelAntwort.Visible = false;
        }
        //------------------------Neu Nachricht-----------------------

        private void bSchicken_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteMitList.Count > 0)
            {
                if (tbSub.Text == "")
                {
                    DialogResult result = MessageBox.Show("Sie haben keine Betreff eingegeben! Möchten Sie trotzdem senden?", "Hoopla", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                string text = rtbNachricht.Text.ToString();
                Nachricht n = new Nachricht(tbSub.Text, DateTime.Now, false);
                foreach (Mitarbeiter m in ausgewaehlteMitList)
                {
                    n.Sender_Empfanger.Add(m);
                }
                panelNeuNachricht.Visible = false;
                Nachricht nn = Nachricht.neuNachricht(n, id, text);
                if (nn.Id != -1)  
                {
                    Nachricht.sNachList.Add(nn);
                    Nachricht.sNachList = Nachricht.sNachList.OrderByDescending(nach => nach.Datum).ToList();
                }
                dgvNLaden(Nachricht.aNachList);
            }
            else
            {
                MessageBox.Show("Wählen Sie minestens einen Empfänger!", "Hoopla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bEmpHinzu_Click(object sender, EventArgs e)
        {
            panelNeuEmpf.Visible = true;
            cbMitarbeiter.Items.Clear();
            foreach (Mitarbeiter m in Mitarbeiter.mitList)
            {
                if ((m.Id == id) || (m.Aktiv==false)) continue;
                if ((m.Aktiv) && (!ausgewaehlteMitList.Contains(m)))
                {
                    cbMitarbeiter.Items.Add(m.Nname + ", " + m.Vname);
                }
            }
            cbMitarbeiter.Items.Add("--Für Alle Mitarbeiter--");
        }

        private void bZurueck_Click(object sender, EventArgs e)
        {
            panelNeuNachricht.Visible = false;
            if (istAngekommen)
            {
                dgvNLaden(Nachricht.aNachList);
            }
            else dgvNLaden(Nachricht.sNachList);
            bLoeschenAusw.Visible = false;
            bUngelesen.Visible = false;
            geixtAnzahl = 0;
            bZurueckNachr.Visible = false;
        }

        private void bEmpAusw_Click(object sender, EventArgs e)
        {
            if (cbMitarbeiter.SelectedIndex >= 0)
            {
                if (cbMitarbeiter.SelectedItem.Equals("--Für Alle Mitarbeiter--"))
                {
                    ausgewaehlteMitList.Clear();
                    lbAusgew.Items.Clear();
                    foreach (Mitarbeiter m in Mitarbeiter.mitList)
                    {
                        if (m.Id == id) continue;
                        if (m.Aktiv) ausgewaehlteMitList.Add(m);
                        lbAusgew.Items.Add(m.Nname + ", " + m.Vname);
                    }
                    cbMitarbeiter.Items.Clear();
                    cbMitarbeiter.Items.Add("--Für Alle Mitarbeiter--");
                }
                else
                {
                    string name = cbMitarbeiter.SelectedItem.ToString();
                    int index = name.IndexOf(",");
                    string nname = name.Substring(0, index);
                    string vname = name.Substring(index + 2);
                    Mitarbeiter m = Mitarbeiter.mitList.Find(
                            delegate (Mitarbeiter mit)
                            {
                                return (mit.Nname == nname && mit.Vname == vname);
                            }
                        );
                    cbMitarbeiter.Items.RemoveAt(cbMitarbeiter.SelectedIndex);
                    lbAusgew.Items.Add(m.Nname + ", " + m.Vname);
                    ausgewaehlteMitList.Add(m);
                }


            }
        }

        private void bLoeschenAusw_Click(object sender, EventArgs e)
        {
            if (lbAusgew.SelectedIndex >= 0)
            {
                string name = lbAusgew.SelectedItem.ToString();
                int index = name.IndexOf(",");
                string nname = name.Substring(0, index);
                string vname = name.Substring(index + 2);
                Mitarbeiter m = Mitarbeiter.mitList.Find(
                        delegate (Mitarbeiter mit)
                        {
                            return (mit.Nname == nname && mit.Vname == vname);
                        }
                    );
                lbAusgew.Items.RemoveAt(lbAusgew.SelectedIndex);
                ausgewaehlteMitList.Remove(m);
                cbMitarbeiter.Items.Add(m.Nname + ", " + m.Vname);
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            panelNeuEmpf.Visible = false;
            lEmpf.Text = "";
            foreach (Mitarbeiter m in ausgewaehlteMitList)
            {
                lEmpf.Text += m.Nname + ", " + m.Vname + "  *  ";
            }
        }

        private void bAntwort_Click(object sender, EventArgs e)
        {
            if (rtbAntwort.Text == "")
            {
                MessageBox.Show("Der text der Nachricht fehlt!", "Hoopla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Nachricht n = new Nachricht("Re:" + aktuelNachricht.Sub, DateTime.Now, false);
            try
            {
                MySqlConnection conn = new MySqlConnection(SERVER);
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select person_id, vname, nname, firma, tel, email, taetigkeit, beginn, stat, aktiv from nachricht join person on person.id=nachricht.sender_id join mitarbeiter on mitarbeiter.person_id=person.id WHERE nachricht.id=@id";
                cmd.Parameters.AddWithValue("@id", aktuelNachricht.Id);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bool aktiv = reader.GetBoolean("aktiv");
                    if (!aktiv)
                    {
                        MessageBox.Show("Der Empfanger arbeitet schon nicht bei der Firma!", "Hoopla", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int id = reader.GetInt32("person_id");
                    string vname = reader.GetString("vname");
                    string nname = reader.GetString("nname");
                    string firma = reader.GetString("firma");
                    string tel = reader.GetString("tel");
                    string email = reader.GetString("email");
                    string taetigkeit = reader.GetString("taetigkeit");
                    DateTime anfang = reader.GetDateTime("beginn");
                    string stat = reader.GetString("stat");
                    n.Sender_Empfanger.Add(new Mitarbeiter(id, vname, nname, firma, tel, email, taetigkeit, anfang,stat, aktiv));
                }
                else
                {
                    MessageBox.Show("Der Empfanger arbeitet schon nicht bei der Firma!", "Hoopla", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString() + "Fehler beim Aktualisieren der Nachrichten. Bitte überprüfen Sie Ihre Verbindung zum Server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Nachricht nn = Nachricht.neuNachricht(n, id, rtbAntwort.Text);
            if (nn.Id != -1)
            {
                Nachricht.sNachList.Add(nn);
                Nachricht.sNachList = Nachricht.sNachList.OrderByDescending(nach => nach.Datum).ToList();
            }
            eingehend();
        }
        //------------------------------Nachricht auswählenmit checkbox-----------
        private void dgvNachricht_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                this.dgvNachricht.CommitEdit(DataGridViewDataErrorContexts.Commit);

                //Check the value of cell
                if ((bool)this.dgvNachricht.CurrentCell.Value == true)
                {
                    geixtAnzahl++;
                }
                else
                {
                    geixtAnzahl--;
                }
                if (geixtAnzahl == 0)
                {
                    panelAusgew.Visible = false;
                    bAusgewLoeschen.Visible = false;
                    bUngelesen.Visible = false;
                    bGelesen.Visible = false;
                }
                else
                {
                    panelAusgew.Visible = true;
                    if (istAngekommen)
                    {
                        bUngelesen.Visible = true;
                        bGelesen.Visible = true;
                    }
                    bAusgewLoeschen.Visible = true;
                }
            }
        }

        private void bAusgewLoeschen_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (DataGridViewRow row in dgvNachricht.Rows)
            {
                DataGridViewCheckBoxCell chkchecking = row.Cells[1] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chkchecking.Value) == true)
                {
                    idList.Add((int)row.Cells[0].Value);
                }
            }
            try
            {
                foreach (int i in idList)
                {
                    MySqlConnection conn = new MySqlConnection(SERVER);
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "delete from nachricht_empfanger where nachricht_id=@id;";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from nachricht where id=@id;";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                if (istAngekommen)
                {
                    Nachricht.aNachrichtListLaden(Nachricht.aNachList, id);
                    dgvNLaden(Nachricht.aNachList);
                }
                else
                {
                    Nachricht.sNachrichtListLaden(id);
                    dgvNLaden(Nachricht.sNachList);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString() + "Fehler beim Aktualisieren der Nachrichten. Bitte überprüfen Sie Ihre Verbindung zum Server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bAusgewLoeschen.Visible = false;
            bUngelesen.Visible = false;
            bGelesen.Visible = false;
            geixtAnzahl = 0;
        }
        private void bGelesen_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (DataGridViewRow row in dgvNachricht.Rows)
            {
                DataGridViewCheckBoxCell chkchecking = row.Cells[1] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chkchecking.Value) == true)
                {
                    idList.Add((int)row.Cells[0].Value);
                }
            }
            try
            {
                foreach (int i in idList)
                {
                    MySqlConnection conn = new MySqlConnection(SERVER);
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "update nachricht set geguckt=true where id=@id;";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                if (istAngekommen)
                {
                    Nachricht.aNachrichtListLaden(Nachricht.aNachList, id);
                    dgvNLaden(Nachricht.aNachList);
                }
                else
                {
                    Nachricht.sNachrichtListLaden(id);
                    dgvNLaden(Nachricht.sNachList);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString() + "Fehler beim Aktualisieren der Nachrichten. Bitte überprüfen Sie Ihre Verbindung zum Server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bAusgewLoeschen.Visible = false;
            bUngelesen.Visible = false;
            bGelesen.Visible = false;
            geixtAnzahl = 0;
        }

        private void bUngelesen_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (DataGridViewRow row in dgvNachricht.Rows)
            {
                DataGridViewCheckBoxCell chkchecking = row.Cells[1] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chkchecking.Value) == true)
                {
                    idList.Add((int)row.Cells[0].Value);
                }
            }
            try
            {
                foreach (int i in idList)
                {
                    MySqlConnection conn = new MySqlConnection(SERVER);
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "update nachricht set geguckt=false where id=@id;";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                if (istAngekommen)
                {
                    Nachricht.aNachrichtListLaden(Nachricht.aNachList, id);
                    dgvNLaden(Nachricht.aNachList);
                }
                else
                {
                    Nachricht.sNachrichtListLaden(id);
                    dgvNLaden(Nachricht.sNachList);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString() + "Fehler beim Aktualisieren der Nachrichten. Bitte überprüfen Sie Ihre Verbindung zum Server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bAusgewLoeschen.Visible = false;
            bUngelesen.Visible = false;
            bGelesen.Visible = false;
            geixtAnzahl = 0;
        }
	}