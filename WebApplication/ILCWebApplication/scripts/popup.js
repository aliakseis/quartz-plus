ModalWin = null;

function openModalWin(url, name , args) {
    if (!ModalWin || ModalWin.closed) {
    if (name && name.indexOf(" ") == -1) {
        ModalWin = window.open(url, name ,args);
    } else {
        ModalWin = window.open(url,"",args);
    }
    } else {
        ModalWin.focus();
    }
    return ModalWin;
}

function showPopup(url, name , ww, wh) {
    if (window.showModalDialog) {
        var result = window.showModalDialog(url, name,
            "dialogWidth:"+ww+"px;dialogHeight:"+wh+"px;dialogLeft:"
            + (screen.availWidth - ww) / 2 + ";dialogTop:" + (screen.availHeight - wh) / 2 + ";resizable:no;status:no");
        if (result === "reload") {
            location.reload(1);
        }
        else if (result) {
            parent.__doPostBack(result[0], result[1]);
        }
    }
    else openModalWin(url, name, "width="+ww+",height="+wh+",toolbar=no,status=no,menubar=no,resizable=no,copyhistory=yes,left="
    + (screen.availWidth - ww) / 2 + ",top=" + (screen.availHeight - wh) / 2);
}

function FocusModalWin() {
    if (ModalWin && !ModalWin.closed) {
        ModalWin.focus();
    }
}

function closeModalWin() {
    try { // Workaround for Microsoft JScript runtime error: Permission denied 
        if (ModalWin && !ModalWin.closed) {
            ModalWin.close();
        }
    } catch(e) {}
}

function modalHandleClose(params) {
    if (window.showModalDialog) {
        returnValue = params;
    }
    else {
        window.opener.parent.__doPostBack(params[0], params[1]);
    }
    self.close();
}