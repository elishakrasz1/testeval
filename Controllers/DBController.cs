using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ItemsWebApp.Controllers
{

    public class DBController
    {

        //DB server IP
        private const string SERVER_NAME = @"BASALT";
        //user name for connecting with db
        public const string DEFAULT_DB_USERNAME = "sa";
        // password for connecting with DB
        private const string DB_PASSWORD = "Password1";
        // defualt dictionary Data Base
        public const string ITEMS_DB = "Items_Db";

        // private const string ITEM_TABLE = "EntityData";

        private const string SUBITEMS_TABLE = "SubItems";

        private const string ITEMS_TABLE = "Items";

        // private const string IMAGE_TABLE = "EntityImage";

        private const string SUBITEMIMAGES_TABLE = "SubItemImages";

        private const string ITEMIMAGES_TABLE = "ItemImages";




        public string ItemsDatabaseConnectionString
        {
            get { return @"Data Source=" + SERVER_NAME + ";database=" + ITEMS_DB + ";User id=" + DEFAULT_DB_USERNAME + ";Password=" + DB_PASSWORD; }
        }

        public DBController()
        {
        }


        public bool SetItemInDB(SubItems item)
        {
            bool success = false;

            try
            {
                if (item.id > 0) //delete first
                {

                    DeleteQuestion(item.item_id);
                                      
                }


                int newID = InsertQuestionText(item.item_id, item.entity_text, item.entity_html);

                if (newID == 0) return false;
                //insert images
                foreach (ItemImages image in item.images )
                {
                    success = InsertQuestionImage(newID, image.tag_id, image.image_file_type, image.image_data);
                    if (!success) return false;
                }
                foreach (SubItems answer in item.images)
                {
                    newID = 0;
                    newID = InsertQuestionText(answer.item_id, answer.entity_text, answer.entity_html, answer.ans_no);

                    if (newID == 0) return false;
                    //insert images
                    foreach (SubItemImages image in answer.images)
                    {
                        success = InsertQuestionImage(newID, image.tag_id, image.image_file_type, image.image_data);
                        if (!success) return false;
                    }
                }



            }
            catch (Exception e)
            {
                return false;
            }


            return success;
        }

        private bool DeleteQuestionImage(int tag_id, string image_file_type, byte[] image_data)
        {
            throw new NotImplementedException();
        }

        #region get
        public Items GetItemTextByID(int item_id)
        {
            Items item = new Items();
            try
            {
                for (int ans_no = 0; ans_no < 5; ans_no++)
                {
                    SubItems data = new SubItems();
                    string entity_text = "";
                    string entity_html = "";
                    int subItemIdinDB = 0;
                    GetQuestionText(item_id, ans_no, out subItemIdinDB, out entity_text, out entity_html);
                    data.entity_text = entity_text;
                    data.entity_html = entity_html;
                    List<SubItemImages> images;
                    GetQuestionImages(subItemIdinDB, out images);
                    data.images = images;
                    if (ans_no == 0)
                        item.question = data;
                    else
                        item.answers.Add(data);
                    
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return item;
        }



        /// <summary>
        /// משיכת טקסט של פריט
        /// </summary>
        /// <param name="itemIDInDB">מספר הפריט במאגר</param>
        /// <param name="answerNo">מספר תשובה או אפס לגזע</param>
        /// <param name="text">טקסט השאלה</param>
        /// <param name="html">הHTML של השאלה</param>
        /// <returns>הצלחה</returns>
        public bool GetQuestionText(int itemIDInDB, int answerNo, out int subItemIdinDB, out string text, out string html)
        {
            text = "";
            html = "";
            subItemIdinDB = 0;
            string connect_string = ItemsDatabaseConnectionString;
            SqlConnection oSqlConnection = new SqlConnection(connect_string);
            try
            {


                // connect to db
                oSqlConnection.Open();
                //get item part
                string getItemQuery = "SELECT id, entity_text, entity_html FROM " + SUBITEMS_TABLE + " WHERE item_id = @item_id AND ans_no = @ans_no";

                SqlCommand sqlcmdItem = new SqlCommand(getItemQuery, oSqlConnection);

                sqlcmdItem.Parameters.Add("@item_id", System.Data.SqlDbType.Int).Value = itemIDInDB;
                sqlcmdItem.Parameters.Add("@ans_no", System.Data.SqlDbType.Int).Value = answerNo;
                SqlDataReader myReadItem = sqlcmdItem.ExecuteReader();
                while (myReadItem.Read())
                {
                    text = (string)myReadItem["entity_text"];
                    html = (string)myReadItem["entity_html"];
                    subItemIdinDB = (int)myReadItem["id"];
                }
                myReadItem.Close();


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oSqlConnection != null && oSqlConnection.State == System.Data.ConnectionState.Open)
                    oSqlConnection.Close();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIDInDB"></param>
        /// <param name="filename"></param>
        /// <param name="imageBlob"></param>
        /// <returns></returns>
        public bool GetQuestionImages(int subItemIDInDB, out List<ItemImages> lisImages)
        {
            lisImages = new List<ItemImages>();
            string connect_string = ItemsDatabaseConnectionString;
            SqlConnection oSqlConnection = new SqlConnection(connect_string);
            try
            {


                // connect to db
                oSqlConnection.Open();
                //get item part
                string getItemQuery = "SELECT id, image_file_type, image_data, tag_id FROM ItemImages WHERE entity_data_ik = @item_id";

                SqlCommand sqlcmdItem = new SqlCommand(getItemQuery, oSqlConnection);

                sqlcmdItem.Parameters.Add("@item_id", System.Data.SqlDbType.Int).Value = subItemIDInDB;
                SqlDataReader myReadItem = sqlcmdItem.ExecuteReader();
                while (myReadItem.Read())
                {
                    ItemImages image = new ItemImages();
                    image.item_id = (int)myReadItem["id"];
                    image.tag_id = (int)myReadItem["tag_id"];
                    image.image_file_type = (string)myReadItem["image_file_type"];
                    image.image_data = (byte[])myReadItem["image_data"];

                    lisImages.Add(image);
                }
                myReadItem.Close();


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oSqlConnection != null && oSqlConnection.State == System.Data.ConnectionState.Open)
                    oSqlConnection.Close();
            }
            return true;
        }

        #endregion //get

        #region insert


        /// <summary>
        /// הכנסת טקסט של פריט
        /// </summary>
        /// <param name="itemIDInDB">מספר הפריט במאגר</param>
        /// <param name="text">הטקסט</param>
        /// <param name="answerNo">מספר תשובה או אפס לגזע</param>
        /// <param name="isFather">האם שאל אב</param>
        /// <returns>הצלחה</returns>
        public int InsertQuestionText(int itemIDInDB, string text, string html, int answerNo = 0)
        {
            string connect_string = ItemsDatabaseConnectionString;
            SqlConnection oSqlConnection = new SqlConnection(connect_string);
            int insertedId = 0;
            try
            {
                
                String InsItems =
                "INSERT INTO SubItems (item_id, entity_text, entity_html, ans_no)" +
                " VALUES(@itemId, @entityText, @entityHtml, @ansNo); SELECT SCOPE_IDENTITY()";

                SqlCommand sqlcmdMap = new SqlCommand(InsItems, oSqlConnection);
                sqlcmdMap.Parameters.Add("@itemId", System.Data.SqlDbType.Int).Value = itemIDInDB;
                sqlcmdMap.Parameters.Add("@entityText", System.Data.SqlDbType.NVarChar).Value = text;
                sqlcmdMap.Parameters.Add("@entityHtml", System.Data.SqlDbType.NVarChar).Value = html;
                sqlcmdMap.Parameters.Add("@ansNo", System.Data.SqlDbType.Int).Value = answerNo;
                insertedId = (Int32)sqlcmdMap.ExecuteScalar();
                sqlcmdMap.Parameters.Clear();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oSqlConnection != null && oSqlConnection.State == System.Data.ConnectionState.Open)
                    oSqlConnection.Close();
            }
            return insertedId;
            
        }


        public bool DeleteQuestion(int itemIDInDB)
        {
            string connect_string = ItemsDatabaseConnectionString;
            SqlConnection oSqlConnection = new SqlConnection(connect_string);
            int success = 0;
            try
            {

                String InsItems =
                "DELETE FROM Items WHERE external_id = @itemId";

                SqlCommand sqlcmdMap = new SqlCommand(InsItems, oSqlConnection);
                sqlcmdMap.Parameters.Add("@itemId", System.Data.SqlDbType.Int).Value = itemIDInDB;
                success = (Int32)sqlcmdMap.ExecuteNonQuery();
                sqlcmdMap.Parameters.Clear();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oSqlConnection != null && oSqlConnection.State == System.Data.ConnectionState.Open)
                    oSqlConnection.Close();
            }
            return (success>0);

        }

        public bool DeleteQuestionImage(int imageIDInDB)
        {
            string connect_string = ItemsDatabaseConnectionString;
            SqlConnection oSqlConnection = new SqlConnection(connect_string);
            int success = 0;
            try
            {

                String InsItems =
                "DELETE FROM ItemImages WHERE id = @itemId";

                SqlCommand sqlcmdMap = new SqlCommand(InsItems, oSqlConnection);
                sqlcmdMap.Parameters.Add("@itemId", System.Data.SqlDbType.Int).Value = imageIDInDB;
                success = (Int32)sqlcmdMap.ExecuteNonQuery();
                sqlcmdMap.Parameters.Clear();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oSqlConnection != null && oSqlConnection.State == System.Data.ConnectionState.Open)
                    oSqlConnection.Close();
            }
            return (success > 0);

        }



        /// <summary>
        /// הכנסת תמונה של השאלה לפי פריט
        /// </summary>
        /// <param name="itemIDInDB">מציין הפריט במאגר</param>
        /// <param name="imageFileType">סוג תמונה</param>
        /// <param name="imageBlob">התמונה</param>
        /// <returns>הצלחה</returns>
        public bool InsertQuestionImage(int itemIDInDB, int idInQuestion, string imageFileType, byte[] imageBlob)
        {
            string connect_string = ItemsDatabaseConnectionString;
            SqlConnection oSqlConnection = new SqlConnection(connect_string);
            int ret = 0;

            try
            {

                String InsItems =
                "INSERT INTO ItemImages (item_id, image_file_type, image_data, tag_id)" +
                " VALUES(@itemId, @fileType, @fileBlob, @tagId)";

                SqlCommand sqlcmdMap = new SqlCommand(InsItems, oSqlConnection);
                sqlcmdMap.Parameters.Add("@itemId", System.Data.SqlDbType.Int).Value = itemIDInDB;
                sqlcmdMap.Parameters.Add("@fileType", System.Data.SqlDbType.NVarChar).Value = imageFileType;
                sqlcmdMap.Parameters.Add("@fileBlob", System.Data.SqlDbType.VarBinary).Value = imageBlob;
                sqlcmdMap.Parameters.Add("@tagId", System.Data.SqlDbType.Int).Value = idInQuestion;
                ret = sqlcmdMap.ExecuteNonQuery();
                sqlcmdMap.Parameters.Clear();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oSqlConnection != null && oSqlConnection.State == System.Data.ConnectionState.Open)
                    oSqlConnection.Close();
            }
            if (ret > 0)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// הכנסת תמונה של השאלה לפי פריט
        /// </summary>
        /// <param name="itemIDInDB">מציין הפריט במאגר</param>
        /// <param name="imageFileType">סוג תמונה</param>
        /// <param name="imageBlob">התמונה</param>
        /// <returns>הצלחה</returns>
        public bool InsertFullItemImage(int itemIDInDB, string imageFileType, byte[] imageBlob)
        {
            string connect_string = ItemsDatabaseConnectionString;
            SqlConnection oSqlConnection = new SqlConnection(connect_string);
            int ret = 0;

            try
            {

                String InsItems =
                "INSERT INTO ItemImages (item_id, image_file_type, image_data)" +
                " VALUES(@itemId, @fileType, @fileBlob)";

                SqlCommand sqlcmdMap = new SqlCommand(InsItems, oSqlConnection);
                sqlcmdMap.Parameters.Add("@itemId", System.Data.SqlDbType.Int).Value = itemIDInDB;
                sqlcmdMap.Parameters.Add("@fileType", System.Data.SqlDbType.NVarChar).Value = imageFileType;
                sqlcmdMap.Parameters.Add("@fileBlob", System.Data.SqlDbType.VarBinary).Value = imageBlob;
                ret = sqlcmdMap.ExecuteNonQuery();
                sqlcmdMap.Parameters.Clear();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oSqlConnection != null && oSqlConnection.State == System.Data.ConnectionState.Open)
                    oSqlConnection.Close();
            }
            if (ret > 0)
            {
                return true;
            }
            else
                return false;
        }
        #endregion
    }



}
