using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace SearchboxPage
{
    public partial class Form1 : Form
    {
        const string _apiUrl = "https://openapi.naver.com/v1/search/news";
        const string _clientId = "I74lzNbMOpmIlEsfaWRO"; 
        const string _clientSecret = "jgntupzVD8"; 

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string results = getResults();
                results = results.Replace("<b>", "");
                results = results.Replace("</b>", "");
                results = results.Replace("&lt;", "<");
                results = results.Replace("&gt;", ">");

                var parseJson = JObject.Parse(results);
                var countsOfDisplay = Convert.ToInt32(parseJson["display"]);
                var countsOfResults = Convert.ToInt32(parseJson["total"]);

                ResultList.Items.Clear();
                for (int i=0; i< countsOfDisplay; i++)
                {
                    ListViewItem item = new ListViewItem((i+1).ToString());
                    
                    var title = parseJson["items"][i]["title"].ToString();
                    title = title.Replace("&quot;", "\"");

                    var description = parseJson["items"][i]["description"].ToString();
                    description = description.Replace("&quot;", "\"");

                    var link = parseJson["items"][i]["link"].ToString();

                    item.SubItems.Add(title);
                    item.SubItems.Add(description);
                    item.SubItems.Add(link);

                    ResultList.Items.Add(item);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        private string getResults()
        {
            string keyword = Searchbox.Text;
            string display = btn_DisplayCount.Value.ToString();
            string sort = "sim";
            if (btn_date.Checked == true)
                sort = "date";

            string query = string.Format("?query={0}&display={1}sort={2}", keyword, display, sort);

            WebRequest request = WebRequest.Create(_apiUrl + query);
            request.Headers.Add("X-Naver-Client-Id", "I74lzNbMOpmIlEsfaWRO");
            request.Headers.Add("X-Naver-Client-Secret", "jgntupzVD8");

            string requestResult = "";
            using (var response = request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        requestResult = reader.ReadToEnd();
                    }
                }
            }
                
            return requestResult;
        }

        private void trackBarDisplayCounts_Scroll(object sender, EventArgs e)
        {
            labelDisplayCounts.Text = btn_DisplayCount.Value.ToString();
        }

        private void textBoxKeyword_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonDate_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonSim_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void labelDisplayCounts_Click(object sender, EventArgs e)
        {

        }
    }
}
