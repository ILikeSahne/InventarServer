using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventarServer
{
    class Database
    {
        /// <summary>
        /// Holds Data about the Database Name, Folder and IDCounter
        /// </summary>
        public DatabaseLocation Loc;

        private string equipmentFolder;

        private List<Equipment> equipments;

        /// <summary>
        /// Saves Values
        /// </summary>
        /// <param name="_Loc">Database Location</param>
        public Database(DatabaseLocation _Loc)
        {
            Loc = _Loc;

            equipmentFolder = Loc.Folder + "equipment/";
            equipments = new List<Equipment>();
        }

        /// <summary>
        /// Creates the Folders for the Database
        /// </summary>
        /// <returns>Returns an Error if it can't create the Folders</returns>
        public DatabaseError CreateDatabase()
        {
            try
            {
                Directory.CreateDirectory(Loc.Folder);
                Directory.CreateDirectory(equipmentFolder);
            }
            catch (Exception e)
            {
                return new DatabaseError(DatabaseErrorType.DATABASE_FOLDER_NOT_FOUND, e);
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Loads the Database-Content from its Folder
        /// </summary>
        /// <returns>Returns an Error if it can't load the Database</returns>
        public DatabaseError LoadDatabase()
        {
            if(!Directory.Exists(Loc.Folder))
                return new DatabaseError(DatabaseErrorType.DATABASE_FOLDER_NOT_FOUND, null);
            if (!Directory.Exists(equipmentFolder))
                return new DatabaseError(DatabaseErrorType.EQUIPMENT_FOLDER_NOT_FOUND, null);
            foreach (string f in Directory.GetFiles(equipmentFolder))
            {
                DatabaseError e = LoadFile(f);
                if (!e)
                    return e;
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Loads a File for the Database
        /// </summary>
        /// <param name="_f">File-Path to load</param>
        /// <returns>Returns an Error if it can't load a File of the Database</returns>
        private DatabaseError LoadFile(string _f)
        {
            try
            {
                string json = File.ReadAllText(_f);
                Equipment e = JsonSerializer.Deserialize<Equipment>(json);
                equipments.Add(e);
            }
            catch (Exception e)
            {
                return new DatabaseError(DatabaseErrorType.EQUIPMENT_FILE_CORRUPTED, e);
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Saves all Files for the Database
        /// </summary>
        /// <returns>Returns an Error if it can't save a File</returns>
        public DatabaseError SaveDatabase()
        {
            InventarServer.GetDatabase().SaveConfig();
            InventarServer.WriteLine("Saving Database: \"{0}\"", Loc.Name);
            foreach(Equipment eq in equipments)
            {
                DatabaseError e = eq.SaveToFile(equipmentFolder);
                if (!e)
                    return e;
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Adds a new Equipment to the Database
        /// </summary>
        /// <param name="e">Equipment to save</param>
        /// <returns>Returns an Error if the Equipment is not valid</returns>
        public DatabaseError AddEquipment(Equipment _e)
        {
            _e.ID = Loc.IDCounter++;
            InventarServer.WriteLine("Adding Equipment to Database \"{0}\", Data: \n{1}", Loc.Name, _e.ToString());
            DatabaseError de = ValidateEquipment(_e);
            if(!de)
                return de;
            equipments.Add(_e);
            SaveDatabase();
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Checks if the Equipment-Data is valid
        /// </summary>
        /// <param name="e">Equipment to validate</param>
        /// <returns>Returns an Error if the Equipment is not valid</returns>
        private DatabaseError ValidateEquipment(Equipment _e)
        {
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }
    }
}
