# TIA_Add-In_VCI_Git_Connector
`This TIA Add-In allows you to connect your VCI workspace at TIA Portal to a Git repository`

---
## Summary
1. [About this Project](#about)
      * 1.1 Requirements
      * 1.2 Supporting Documentations
2. [How to Use (Installation)](#installation)
      * 2.1 Installation Requirements
      * 2.2 Add-In Download
      * 2.3 Save .addin File to Correct Location
      * 2.4 Activation
      * 2.5 Configuring the Workspace
3. [Functions Documentation](#functions)
    * [Add]
    * [Commit]
  

---
## <a name="about"></a> 1. About this Project
This is a TIA Add-In, what is a way to embed functions to your TIA Portal. No additional external applications are required to run TIA Add-Ins (See how to use it [here](#About)).

This project is based on a *Siemens AG - SIMATIC Systems Support project:* ***109773999_TIA_Add-In_VCI_Git_Connector_1.0.0_CODE***, to witch functions were added and modified.

With this Add-In you can connect the *Version Control Interface* of TIA with a git repository and then use all git's power on your project.

### 1.1 Requirements
* Basic knowledge on Git
* Basic knowledge on TIA

### 1.2 Supporting Documentations
* [VCI Documentation | Using TIA Portal Version Control Interface](https://support.industry.siemens.com/cs/mdm/109773506?c=129126268427&lc=en-BR)
* [Git Documentation](https://git-scm.com/)
* [Add-In Documentation | Extending TIA Portal functions with add-ins](https://support.industry.siemens.com/cs/mdm/109773506?c=128474251915&lc=en-BR)

---
## <a name="installation"></a> 2. How to Use (Installation)
Follow the steps below

### 2.1 Installation Requirements
* TIA V16, V17 or V18 installed - [V18 TRIAL Download](https://support.industry.siemens.com/cs/document/109807109/simatic-step-7-incl-safety-s7-plcsim-and-wincc-v18-trial-download?dti=0&lc=en-WW)
* Git installed - [Latest source Release Download](https://git-scm.com/downloads)

### 2.2 Add-In Download
* Version 1.0.0 - [VCIGitConnector.addin](https://github.com/IgorCoraine/TIA_Add-In_VCI_Git_Connector/blob/master/VCI%20Git%20Connector/bin/Debug/Siemens.VCIGitConnector.addin)

### 2.3 Save .addin File to Correct Location
No installation is required to run the Add-In. You just have to **copy the .addin file downloaded above to your AddIns folder**.

The AddIns folder is finded inside your TIA Portal Installation directory. The standard location is 
<br>`C:\Program Files\Siemens\Automation\Portal V17\AddIns`

*When you paste it you will be asked to administrator permission, you just have to confirm if you are already logged as windows administrator.*

### 2.4 Activation
After coping the file, open your TIA Portal and open the *project view*. On the right of the screen open the *Add-ins* tab, select VCIGitConnector.addin and change the status to *Activate*. You will be asked to apply permissions, click *Yes* to Activate the Add-In.

![activation image](docImages/installation.png)

### 2.5 Configuring the Workspace
If you followed the steps above, your Add-In is active on your TIA Portal, independently from the TIA project.

To use git on your project, create a *VCI workspace*, configure the repository folder, and select Git as the *Version control add-in*.

![configuring image](docImages/configuration.png)

---
## <a name="functions"></a> 3. Functions Documentation
**To use any functions you must have any file to the configured repository and right click with your mouse**

![navigation image](docImages/navigation.png)

### 3.1 Add

### 3.2 Commit

---
on edition...
