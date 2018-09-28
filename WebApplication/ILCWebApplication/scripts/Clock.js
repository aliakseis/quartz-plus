//basic Calendar Object
var ClockZIndex = 1000;

function Clock (inputName, divId, imagePath, varName, picId) {
    this.inputName = new Object(inputName);
    this.divId = divId;
    this.imagePath = imagePath;
    if (picId) {
        this.picId = picId; // id of clock image
    } else {
        this.picId = null;
    }
    this.varName = varName;
    this.hideSelects = null;
    
    this.getTime = getTime;
    this.writeTime = writeTime;
    
    this.changeHour = changeHour;
    this.changeTen = changeTen;
    this.changeMinute = changeMinute;
    this.formatTime = formatTime;
    this.changeAmpm = changeAmpm;
    
    this.writeClockTable = writeClockTable;
    
    this.showHideClock = showHideClock;
    this.clearSpaces = clearSpaces;
    
    this.writeToInput = writeToInput;
    
    this.hour = new Number();
    this.ten = new Number();
    this.minute = new Number();
    this.ampm = new String();
    this.isVisible = false;
    this.timeOutVar = 0;
    
    this.mouseOver = mouseOverClock;
    this.mouseOut = mouseOutClock;
    
    this.setDateTimeFlag = false;
    this.setEndDateTime = setDateTimeClock;
}

function setDateTimeClock() {
    this.setDateTimeFlag = true;
}

function mouseOutClock() {
    this.timeOutVar = window.setTimeout("hideClock('" + this.divId + "','" + this.varName + "')",1000);
}

function mouseOverClock() {
    if (this.timeOutVar != null) {
        window.clearTimeout(this.timeOutVar);
    }
}

function hideClock(divId, varName) {
    document.getElementById(divId).style.visibility = "hidden";
    document.getElementById(divId).style.display = "none";
    eval(varName).isVisible = false;
    // showing the select objs that were hidden
    var obj = eval(varName);
    if (obj.hideSelects != null && navigator.appName != "Netscape") {
        for(var i = 0; i < obj.hideSelects.length; i++) {
            document.getElementById(obj.hideSelects[i]).style.visibility = "visible";
        }
    }
}

function showHideClock() {
    if (this.isVisible) {
        this.writeTime();
        document.getElementById(this.divId).style.visibility = "hidden";
        document.getElementById(this.divId).style.display = "none";
        this.isVisible = false;
        if (this.hideSelects != null && navigator.appName != "Netscape") {
            for(var i = 0; i < this.hideSelects.length; i++) {
                document.getElementById(this.hideSelects[i]).style.visibility = "visible";
            }
        }
    } else {
        this.getTime();
        this.writeClockTable();
        this.writeTime();
                
        var control = this.inputName;
		var pic = document.getElementById(this.picId);
    	var topCoord = pic.offsetTop + control.offsetHeight + 1;
    	var leftCoord = pic.offsetLeft - control.offsetWidth;
    	
    	if(navigator.appName == "Netscape"){
            var maxBottom = window.innerHeight;
		} else {
            var maxBottom = document.body.clientHeight;
		}
		if ((topCoord + 56) > maxBottom){
        	topCoord = topCoord - 45;
        }
        
        document.getElementById(this.divId).style.left = leftCoord + "px";
        document.getElementById(this.divId).style.top = topCoord + "px";
        
        if (this.hideSelects != null && navigator.appName != "Netscape") {
            for (var i = 0; i < this.hideSelects.length; i++) {
                document.getElementById(this.hideSelects[i]).style.visibility = "hidden";
            }
        }
        
        document.getElementById(this.divId).style.visibility = "visible";
        document.getElementById(this.divId).style.display = "block";
        this.isVisible = true;
        this.timeOutVar = window.setTimeout("hideClock('" + this.divId + "','" + this.varName + "')",3000);
    }
}

function getTime() {
    var time = this.clearSpaces(this.inputName.value);
    if (time == "") {
        var today = new Date();
        var hours = today.getHours();
        var minutes = today.getMinutes();
        
        if (minutes <= 30) {
            this.hour = hours;
            this.ten = 3;
            this.minute = 0;
        } else {
            this.hour = hours + 1;
            this.ten = 0;
            this.minute = 0;
        }
        
        if (this.hour == 0 || this.hour == 24) {
            this.hour = 12;
            this.ampm = "AM";
        } else if (this.hour > 0 && this.hour <= 11) {
            this.ampm = "AM";
        } else if (this.hour == 12) {
            this.ampm = "PM";
        } else {
            this.hour -= 12;
            this.ampm = "PM";
        }
    } else {
        var colon = time.indexOf(":");
        this.hour = new Number(time.substring(0, colon));
        var space = time.indexOf(" ", colon);
        this.ten = new Number(time.substring(colon+1, colon+2));
        this.minute = new Number(time.substring(colon+2, space));
        this.ampm = time.substring(space+1, time.length);
    }
}

function writeTime() {
    document.getElementById(this.divId + "Ampm").innerHTML = this.ampm;
    document.getElementById(this.divId + "Min").innerHTML = this.minute;
    document.getElementById(this.divId + "Ten").innerHTML = this.ten;
    document.getElementById(this.divId + "Hour").innerHTML = this.hour;
}

function writeToInput() {
    var time = ((this.hour < 10) ? "0" + this.hour : this.hour) + ":" + this.ten + "" + this.minute + " " + this.ampm;
    this.inputName.value = time;
    
    ValidatorOnChange( { srcElement : this.inputName } );
    
    if (this.setDateTimeFlag) {
        SaveTimeInterval(this.inputName);
    }
    this.showHideClock();
}


function changeMinute(val) {
    this.minute += val;
    
    if (this.minute >= 10) {
        this.minute -= 10;
        this.changeTen(1);
    } else if (this.minute < 0) {
        this.minute += 10;
        this.changeTen(-1)
    }
    document.getElementById(this.divId + "Min").innerHTML = this.minute;
}

function changeTen(val)
{
    this.ten += val;
    
    if (this.ten >= 6) {
        this.ten -= 6;
        this.changeHour(1);
    } else if (this.ten < 0) {
        this.ten += 6;
        this.changeHour(-1);
    }
    document.getElementById(this.divId + "Ten").innerHTML = this.ten;
}

function changeHour(val) {
    this.hour += val;
    if (this.hour > 12) {
        this.hour -= 12;
    } else if (this.hour < 1) {
        this.hour += 12;
    }
    document.getElementById(this.divId + "Hour").innerHTML = this.hour;
}

function changeAmpm() {
    this.ampm = (this.ampm == "AM") ? "PM" : "AM";
    document.getElementById(this.divId + "Ampm").innerHTML = this.ampm;
}


function formatTime(which) {
    var formatted = new String(which);
    if (formatted.length == 1) {
        formatted = "0" + formatted;
    }
    return formatted;
}


function writeClockTable() {
    var html = "";
    var src = (navigator.userAgent.indexOf("Netscape6") != -1)
    ? "src=\"javascript: void(0)\""
            : "src=\"" + this.imagePath + "spacer.gif\"";
    html += "<div id=\"iframeDiv\" style=\"position: absolute; left: -2px; z-index:" 
        + (ClockZIndex + 1) + "; width=\"" + this.width 
        + "\"><iframe width=\"100%\" height=\"54\" frameborder=\"0\" " + src 
        + " style=\"display: inherit\"></iframe></div>";
    html += "<div id=\"clockDiv\" style=\"position: relative; top: 0px; z-index:" + (ClockZIndex + 2) + "\">";
    html += "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"110\" class=\"clockBorder\">";
    html += "<tr class=\"clockBack\" height=\"20\">";
    html += "   <td class=\"clockText\" align=\"center\"><span id=\"" + this.divId + "Hour\">&nbsp;</span></td>";
    html += "   <td class=\"clockText\" align=\"center\">:</td>";
    html += "   <td class=\"clockText\" align=\"center\"><span id=\"" + this.divId + "Ten\">&nbsp;</span></td>";
    html += "   <td class=\"clockText\" align=\"center\"><span id=\"" + this.divId + "Min\">&nbsp;</span></td>";
    html += "   <td class=\"clockText\" align=\"center\"><a href=\"javascript:" + this.varName + ".changeAmpm();\" class=\"clockLink\"><span id=\"" + this.divId + "Ampm\">&nbsp;</span></a></td>";
    html += "</tr>";
    html += "<tr>";
    html += "   <td align=\"center\" width=\"30\"><a href=\"javascript:" + this.varName + ".changeHour(1);\"><img src=\"" + this.imagePath + "icon_arrowUp.gif\" border=\"0\"></a></td>";
    html += "   <td width=\"10\">&nbsp;</td>";
    html += "   <td align=\"center\" width=\"25\"><a href=\"javascript:" + this.varName + ".changeTen(1);\"><img src=\"" + this.imagePath + "icon_arrowUp.gif\" border=\"0\"></a></td>";
    html += "   <td align=\"center\" width=\"25\"><a href=\"javascript:" + this.varName + ".changeMinute(1);\"><img src=\"" + this.imagePath + "icon_arrowUp.gif\" border=\"0\"></a></td>";
    html += "   <td width=\"50\" rowspan=\"2\" align=\"center\" style=\"vertical-align:middle\"><input type=\"button\" style=\"width:34\" value=\"OK\" onclick=\"" + this.varName + ".writeToInput();\" /></td>";
    html += "</tr>";
    html += "<tr>";
    html += "   <td align=\"center\"><a href=\"javascript:" + this.varName + ".changeHour(-1);\"><img src=\"" + this.imagePath + "icon_arrowDown.gif\" border=\"0\"></a></td>";
    html += "   <td>&nbsp;</td>";
    html += "   <td align=\"center\"><a href=\"javascript:" + this.varName + ".changeTen(-1);\"><img src=\"" + this.imagePath + "icon_arrowDown.gif\" border=\"0\"></a></td>";
    html += "   <td align=\"center\"><a href=\"javascript:" + this.varName + ".changeMinute(-1);\"><img src=\"" + this.imagePath + "icon_arrowDown.gif\" border=\"0\"></a></td>";
    html += "</tr>";
    html += "</table>";
    html += "</div>";   
    document.getElementById(this.divId).innerHTML = html;
}
