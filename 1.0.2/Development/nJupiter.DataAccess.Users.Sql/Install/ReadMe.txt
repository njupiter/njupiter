- While developing a new version:
	* every time something in the database is changed, make a change script and put the contents of it in the end 
	  of the file named "Update.DefaultDatabase.<OLD VERSION>.to.Current.sql".
  
- When making a new version:
	* copy the contents of the files in "Install\Database" in order of how they are numbered to a new file in 
	  "Install\Database\Old" and name it "DefaultDatabase.<VERSION>.sql".
    * rename "Update.DefaultDatabase.<OLD VERSION>.to.Current.sql" in "Install\Database\Updates" to
      "Update.DefaultDatabase.<OLD VERSION>.to.<VERSION>.sql"
    * make a new empty file named "Update.DefaultDatabase.<VERSION>.to.Current.sql" in "Install\Database\Updates".