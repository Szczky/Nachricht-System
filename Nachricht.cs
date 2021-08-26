using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Org.BouncyCastle.Asn1.Nist;

namespace Secudrive
{
    class Nachricht
    {
        public int Id { get; set; }
        public string Sub { get; }
        public DateTime Datum { get; }
        public List<Mitarbeiter> Sender_Empfanger { set; get; } = new List<Mitarbeiter>();
        public bool Geguckt { set; get; }
       
        public static List<Nachricht> aNachList = new List<Nachricht>();    //Angekommene
        public static List<Nachricht> sNachList = new List<Nachricht>();    //Sendete
        
        //....
        
        public Nachricht()
        {
            this.Id = -1;
        }

        public Nachricht(int id, string sub, DateTime datum, bool geguckt)
        {
            this.Id = id;
            this.Sub = sub;
            this.Datum = datum;
            this.Geguckt = geguckt;

        }
        public Nachricht(string sub, DateTime datum, bool geguckt)
        {
            this.Id = -1;
            this.Sub = sub;
            this.Datum = datum;
            this.Geguckt = geguckt;

        }
        public static void aNachrichtListLaden(List<Nachricht> nachList, int eId)
        {
            nachList.Clear();
            try
            {
                MySqlConnection conn = new MySqlConnection(Form1.SERVER);
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select nachricht.id, sub, datum, geguckt from nachricht join nachricht_empfanger on nachricht.id=nachricht_empfanger.nachricht_id WHERE nachricht_empfanger.mitarbeiter_id = @id order by nachricht.datum DESC ";     //select nachricht.id, sub, datum,  geguckt, mitarbeiter.person_id, person.vname, person.nname, person.tel, person.email, mitarbeiter.taetigkeit, mitarbeiter.beginn, mitarbeiter.stat, mitarbeiter.aktiv, person.firma from nachricht join nachricht_empfanger on nachricht.id = nachricht_empfanger.nachricht_id join person on person.id = nachricht.sender_id join mitarbeiter on mitarbeiter.person_id = person.id WHERE nachricht_empfanger.mitarbeiter_id = @id order by nachricht.datum DESC; ";
                cmd.Parameters.AddWithValue("@id", eId);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string sub = reader.GetString("sub");
                    DateTime datum = reader.GetDateTime("datum");
                    bool geguckt = reader.GetBoolean("geguckt");
                    Nachricht n = new Nachricht(id, sub, datum, geguckt);
                    MySqlConnection conn2 = new MySqlConnection(Form1.SERVER);
                    conn2.Open();
                    MySqlCommand cmd2 = conn2.CreateCommand();
                    cmd2.CommandText = "select mitarbeiter.person_id, person.vname, person.nname, person.tel, person.email, mitarbeiter.taetigkeit, mitarbeiter.beginn, mitarbeiter.stat, mitarbeiter.aktiv, person.firma from nachricht join nachricht_empfanger on nachricht.id = nachricht_empfanger.nachricht_id join person on person.id = nachricht.sender_id join mitarbeiter on mitarbeiter.person_id = person.id WHERE nachricht.id=@n_id; ";
                    cmd2.Parameters.AddWithValue("@n_id", id);
                    cmd2.Prepare();
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    int person_id;
                    string vname;
                    string nname;
                    string tel;
                    string email;
                    string taetigkeit;
                    DateTime anfang;
                    string firma;
                    bool aktiv;
                    string stat;
                    if (reader2.Read())
                    {
                        person_id = reader2.GetInt32("person_id");
                        vname = reader2.GetString("vname");
                        nname = reader2.GetString("nname");
                        tel = reader2.GetString("tel");
                        email = reader2.GetString("email");
                        taetigkeit = reader2.GetString("taetigkeit");
                        anfang = reader2.GetDateTime("beginn");
                        firma = reader2.GetString("firma");
                        aktiv = reader2.GetBoolean("aktiv");
                        stat = reader2.GetString("stat");
                    }
                    else
                    {
                        person_id = -10;
                        vname = "gelöscht]";
                        nname = "[Mitarbeiter";
                        tel = "";
                        email = "";
                        taetigkeit = "";
                        anfang = new DateTime(0000);
                        firma = "";
                        aktiv = false;
                        stat = "";
                    }
                    reader2.Close();
                    conn2.Close();
                    Mitarbeiter m = new Mitarbeiter(person_id, vname, nname, firma, tel, email, taetigkeit, anfang,stat,  aktiv);
                    n.Sender_Empfanger.Add(m);
                    nachList.Add(n);
                }
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public static void sNachrichtListLaden(int eId)  //gesendete
        {
            sNachList.Clear();
            try
            {
                MySqlConnection conn = new MySqlConnection(Form1.SERVER);
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select id, sub, datum,  geguckt from nachricht WHERE nachricht.sender_id =1 order by nachricht.datum DESC; ";
                cmd.Parameters.AddWithValue("@id", eId);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string sub = reader.GetString("sub");
                    DateTime datum = reader.GetDateTime("datum");
                    bool geguckt = reader.GetBoolean("geguckt");
                    Nachricht n = new Nachricht(id, sub, datum, geguckt);
                    MySqlConnection conn2 = new MySqlConnection(Form1.SERVER);
                    conn2.Open();
                    MySqlCommand cmd2 = conn2.CreateCommand();
                    cmd2.CommandText = "select person.id, person.vname, person.nname, person.firma, person.tel, person.email, mitarbeiter.taetigkeit, mitarbeiter.beginn, mitarbeiter.stat, mitarbeiter.aktiv from person join mitarbeiter on mitarbeiter.person_id = person.id join nachricht_empfanger on nachricht_empfanger.mitarbeiter_id = mitarbeiter.person_id join nachricht on nachricht.id = nachricht_empfanger.nachricht_id where nachricht.sender_id = @senderId and nachricht.id=@nachrichtId; ";
                    cmd2.Parameters.AddWithValue("@nachrichtId", n.Id);
                    cmd2.Parameters.AddWithValue("@senderId", eId);
                    cmd2.Prepare();
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        int person_id = reader2.GetInt32("id");
                        string vname = reader2.GetString("vname");
                        string nname = reader2.GetString("nname");
                        string tel = reader2.GetString("tel");
                        string email = reader2.GetString("email");
                        string taetigkeit = reader2.GetString("taetigkeit");
                        DateTime anfang = reader2.GetDateTime("beginn");
                        string firma = reader2.GetString("firma");
                        bool aktiv = reader2.GetBoolean("aktiv");
                        string stat = reader2.GetString("stat");
                        Mitarbeiter m = new Mitarbeiter(person_id, vname, nname, firma, tel, email, taetigkeit, anfang, stat, aktiv);
                        n.Sender_Empfanger.Add(m);
                    }
                    reader2.Close();
                    cmd2.CommandText = "select count(*) as anzahl from nachricht_empfanger where nachricht_id=@na_id and mitarbeiter_id IS NULL ";
                    cmd2.Parameters.AddWithValue("@na_id", id);
                    cmd2.Prepare();
                    reader2 = cmd2.ExecuteReader();
                    int anzahl = 0;
                    while (reader2.Read())
                    {
                        anzahl = reader2.GetInt32("anzahl");
                        
                    }
                    for (int i = 0; i < anzahl; i++)
                    {
                        int person_id = -10;
                        string vname = "gelöscht]";
                        string nname = "[Mitarbeiter";
                        string tel = "";
                        string email = "";
                        string taetigkeit = "";
                        DateTime anfang = new DateTime(0000);
                        string firma = "";
                        bool aktiv = false;
                        string stat = "";
                        Mitarbeiter m = new Mitarbeiter(person_id, vname, nname, firma, tel, email, taetigkeit, anfang, stat, aktiv);
                        n.Sender_Empfanger.Add(m);
                    }
                    reader2.Close();
                    conn2.Close();
                    sNachList.Add(n);
                }
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public static Nachricht neuNachricht(Nachricht n, int eId, string text)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(Form1.SERVER);
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "insert into nachricht (sub, ntext,datum,sender_id) Values (@sub, @ntext,@datum,@eId)";
                cmd.Parameters.AddWithValue("@sub", n.Sub);
                cmd.Parameters.AddWithValue("@ntext", text);
                cmd.Parameters.AddWithValue("@datum", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                cmd.Parameters.AddWithValue("@eid", eId);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                int nachrichtId = (int)cmd.LastInsertedId;
                foreach (Mitarbeiter m in n.Sender_Empfanger)
                {
                    MySqlConnection conn2 = new MySqlConnection(Form1.SERVER);
                    conn2.Open();
                    MySqlCommand cmd2 = conn2.CreateCommand();
                    cmd2.CommandText = "insert into nachricht_empfanger Values (@nachrichtId, @mitarbeiterId); ";
                    cmd2.Parameters.AddWithValue("@nachrichtId", nachrichtId);
                    cmd2.Parameters.AddWithValue("@mitarbeiterId", m.Id);
                    cmd2.Prepare();
                    cmd2.ExecuteNonQuery();
                    conn2.Close();
                }
                n.Id = nachrichtId;
                conn.Close();
                return n;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Nachricht nn = new Nachricht(-1, "", DateTime.Now, true);
                return nn;
            }
        }
        public string textNachtricht()
        {
            string text = "";
            try
            {
                MySqlConnection conn = new MySqlConnection(Form1.SERVER);
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT ntext FROM `nachricht` WHERE id=@nId";
                cmd.Parameters.AddWithValue("@nId", Id);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    text = reader.GetString("ntext");
                }
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return text;
        }
        public void gelesen()
        {
            if (!Geguckt)
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(Form1.SERVER);
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE nachricht SET geguckt=true WHERE id=@nId";
                    cmd.Parameters.AddWithValue("@nId", Id);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Geguckt = true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Fehler\nProblem mit der Datenbankverbindung\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
