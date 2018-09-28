// basic Calendar Object
var CalendarZIndex = 1000;

function CalendarExt( divId, varName, fieldName, imagePath, picId )
{
    this.fireOnDateChange = null;
    this.divId = divId; // id of div being written to
    this.varName = varName; // var name of Calendar object    
    this.inputName = fieldName;
    this.imagePath = imagePath; // path to images directory
	if (picId){
		this.picId = picId; // id of calendar image
	} else {
		this.picId = null;
	}
	/* isShowCalendar flag. True if it granted to show calendar*/
	this.isShowCalendar = true;

	/* Function for alternative execution when user clicks calendar*/
	this.onClickAltFunc = null;

    this.setTableAttributes = setTableAttributes;

    this.writeDateToInput = writeDateToInput;

    this.showHideCalendar = showHideCalendar;
    this.mouseOver = mouseOver;
    this.mouseOut = mouseOut;

    this.setDefaults = setDefaults;

    this.removeYearArrows = removeYearArrows;
	this.setIsShowCalendar = setIsShowCalendar;

    this.changeMonth = changeMonth; //calls changeTheMonth(), calls writeHtml()
    this.changeYear = changeYear; // calls changeTheYear(), calls writeHtml()
    this.changeTheMonth = changeTheMonth; // changes the month
    this.changeTheYear = changeTheYear; // changes the year
    this.getFeb = getFeb; // returns the proper number of days in feb for a year
    this.setMonthDays = setMonthDays; // initializes the monthDays array, uses getFeb()
    this.setTheDate = setTheDate; // accepts a date string and sets the Calendar's date
    this.writeHtml = writeHtml; // builds a string of HTML and writes it to the div
    this.writeDays = writeDays; // builds a string of HTML and returns it to writeHtml()

    this.clearSpaces = clearSpaces;

    this.changedDataFlag = false;
    this.setChangedDataFlag = setChangedDataFlag;
}

CalendarExt.prototype.days = new Array("Su","Mo","Tu","We","Th","Fr","Sa");
CalendarExt.prototype.months = new Array("January","February","March","April","May","June","July","August","September","October","November","December");
CalendarExt.prototype.monthDays = new Array();
CalendarExt.prototype.width = null;
CalendarExt.prototype.cellpadding = null;
CalendarExt.prototype.cellspacing = null;
CalendarExt.prototype.inputName = null;
CalendarExt.prototype.timeOutVar = null;
CalendarExt.prototype.month = null;
CalendarExt.prototype.day = null;
CalendarExt.prototype.year = null;
CalendarExt.prototype.removeYearArrowsFlag = null;

/* Sets isShowCalendar flag*/
function setIsShowCalendar(showFlag, onClickAltFunc){
	this.isShowCalendar = showFlag;
	this.onClickAltFunc = onClickAltFunc
}

function setChangedDataFlag()
{
    this.changedDataFlag = true;
}

function mouseOut()
{
    this.timeOutVar = window.setTimeout("hideCalendar('" + this.divId + "','" + this.varName + "')",1000);
}
function mouseOver()
{
    if( this.timeOutVar != null )
        window.clearTimeout( this.timeOutVar );
}
function hideCalendar( divId, varName )
{
    document.getElementById( divId ).style.visibility = "hidden";

    // showing the select objs that were hidden
    var obj = eval(varName);
    if( obj.hideSelects != null && navigator.appName == "Netscape" ){
        for( var i=0; i<obj.hideSelects.length; i++ ){
            if (document.getElementById(obj.hideSelects[i])){
                document.getElementById(obj.hideSelects[i]).style.display = "block";
            }
        }
    }
}


function writeDateToInput( month, day, year )
{
    month++;
    var strDate = new String( (month<10?"0"+month:month) + "/" + (day<10?"0"+day:day) + "/" + year );
    var control = eval(this.inputName);
    control.value = strDate;
    
    ValidatorOnChange( { srcElement : control } );

    if (this.fireOnDateChange != null){
        this.fireOnDateChange();
    }

    this.showHideCalendar();
}

function changeYear( number )
{
    this.changeTheYear( number );
    this.writeHtml();
}

function changeMonth( number )
{
    this.changeTheMonth( number );
    this.writeHtml();
}

function changeTheYear( number )
{
    this.year += number;
}

function changeTheMonth( number )
{
    if( this.month + number < 0 )
    {
        this.month = 12 + number + this.month;
        this.changeYear(-1);
    }
    else if( this.month + number > 11 )
    {
        this.month = this.month + number - 12;
        this.changeYear(1);
    }
    else
        this.month += number;
}

function setMonthDays()
{
    this.monthDays = new Array(31,this.getFeb(),31,30,31,30,31,31,30,31,30,31);
}
// returns the proper number of days in feb for the given year
// if year divisible by 100 AND 400, its a leapyear (1600,2000)
// if year is divisible by 4 BUT NOT 100 its a leapyear (1996,2004)
// if year is divisible by 4 AND by 100 its NOT a leapyear (1500,1800);
function getFeb()
{
    if( this.year % 4 == 0 && this.year % 100 != 0 )
        return 29;
    else if( this.year % 100 == 0 && this.year % 400 == 0 )
        return 29;
    else
        return 28;
}

function removeYearArrows()
{
    this.removeYearArrowsFlag = true;
}

function setTableAttributes(width, cellpadding, cellspacing)
{
    this.width = width;
    this.cellpadding = cellpadding;
    this.cellspacing = cellspacing;
    document.getElementById( this.divId ).style.width = width;
}

function setDefaults()
{
    if( this.width == null )
        this.width = "161";
    if( this.cellpadding == null )
        this.cellpadding = "0";
    if( this.cellspacing == null )
        this.cellspacing = "0";

    if( this.removeYearArrowsFlag == null )
        this.removeYearArrowsFlag = false;

    this.setMonthDays();
}

function setTheDate( dateString )
{
    var string = clearSpaces( dateString );
    var arrayString = new Array();

    if( string.indexOf("-") != -1 )
        arrayString = string.split("-");
    else if( string.indexOf("/") != -1 )
        arrayString = string.split("/");

    if( arrayString.length != 3 )
        string = "";
    else
    {
        for( var i=0; i<arrayString.length; i++ )
        {
            var check = new Number( arrayString[i] );
            if( check == "NaN" )
            {
                string = "";
                break;
            }
        }
    }

    if( string != "" )
        var objDate = new Date( string );
    else
        var objDate = new Date();

    this.month = objDate.getMonth();
    this.day = objDate.getDate();
    this.year = objDate.getFullYear();
    if( arrayString[2] && arrayString[2] < 100 )
        this.year += 100; // ie and ns both will list 02 as 1902
}


function showHideCalendar()
{
    var isHidden = (document.getElementById( this.divId ).style.visibility=="hidden")?true:false;

	/* case when it is forbidden to show calendar */
	if (!this.isShowCalendar) {
		// case when alternative function for calendar onClick event execute
		if (this.onClickAltFunc != null){
			eval(this.onClickAltFunc + "()");
			return
		}
		return;
	}

    if( isHidden )
    {
        CalendarZIndex++;
        this.setTheDate( eval( this.inputName ).value );
        this.writeHtml();
		
        var control = eval(this.inputName);
		var pic = document.getElementById(this.picId);
    	var topCoord = pic.offsetTop + control.offsetHeight;
    	var leftCoord = pic.offsetLeft - control.offsetWidth;
    	
    	if(navigator.appName == "Netscape"){
            var maxBottom = window.innerHeight;
		} else {
            var maxBottom = document.body.clientHeight;
		}
		if ((topCoord + 115) > maxBottom){
        	topCoord = topCoord - 100;
        }

    	document.getElementById( this.divId ).style.left = leftCoord + "px";
    	document.getElementById( this.divId ).style.top = topCoord + "px";
		
        document.getElementById( this.divId ).style.zIndex = CalendarZIndex;
        // Hiding select objs specified in IE
        if( this.hideSelects != null && navigator.appName == "Netscape" )
        {
            for( var i=0; i<this.hideSelects.length; i++ ) {
               if (document.getElementById(this.hideSelects[i])) {
            		document.getElementById(this.hideSelects[i]).style.display = "none";
               }
            }
        }
        document.getElementById( this.divId ).style.visibility = "visible";
        document.getElementById( this.divId ).style.display = "block";
        this.timeOutVar = window.setTimeout("hideCalendar('" + this.divId + "','" + this.varName + "')",3000);
    }
    else
    {
        document.getElementById( this.divId ).style.visibility = "hidden";
        document.getElementById( this.divId ).style.display = "none";
    }
}

function writeDays()
{
    var html = new String();

    var objNewDate = new Date( this.year, this.month, 1 );
    
    var weekday = objNewDate.getDay(); // get the weekday of the first of the month

    var objTodayDate = new Date();
    var todayMonth = objTodayDate.getMonth();
    var todayDay = objTodayDate.getDate();
    var todayYear = objTodayDate.getFullYear();

    var date = 1; // the date written to the calendar
    var start = false; // when to start writing the date into the table
    var finished = false; // when to stop writing the date into the table

    for( var i=0; i<6; i++ )
    {
        html += "<tr>";
        for( var j=0; j<7; j++ )
        {
            if( weekday == j && !finished )
                start = true;

            if( start == true )
            {
                var theLink = "javascript:" + this.varName + ".writeDateToInput(" + this.month + "," + date + "," + this.year + ");";

                if( this.month == todayMonth && this.year == todayYear && date == todayDay )
                    html += "<td class=\"calToday\" align=\"center\"><a href=\"" + theLink + "\" class=\"calLinkToday\">" + date + "</a></td>";
                else if( date == this.day )
                    html += "<td class=\"calSelected\" align=\"center\"><a href=\"" + theLink + "\" class=\"calLink\">" + date + "</a></td>";
                else
                    html += "<td class=\"calDay\" align=\"center\"><a href=\"" + theLink + "\" class=\"calLink\">" + date + "</a></td>";
                date++;
                if( date > this.monthDays[this.month] ) // no more days in this month
                {
                    start = false;
                    finished = true;
                }
            }
            else
                html += "<td class=\"calDay\">&nbsp;</td>";

        }
        html += "</tr>";
    }

    return html;
}

function writeHtml()
{
    var objDate = new Date();

    // setup date
    if( this.month != null )
        objDate.setMonth( this.month );
    else
        this.month = objDate.getMonth();

    if( this.day != null )
        objDate.setDate( this.day );
    else
        this.day = objDate.getDate();

    if( this.year != null )
        objDate.setYear( this.year );
    else
    {
        this.year = objDate.getFullYear();
    }

    this.setDefaults();

    var prevYear = "";
    var nextYear = "";
    var middleWidth = "98%";

    if( this.removeYearArrowsFlag == false )
    {
        prevYear = "<td class=\"calTop\" align=\"left\" width=\"1%\"><a href=\"javascript:" 
        	+ this.varName + ".changeYear(-1);\"><img src=\"" + this.imagePath 
        	+ "prevyear.gif\" alt=\"Previous Year\" border=0></a></td>";
        nextYear = "<td class=\"calTop\" align=\"left\" width=\"1%\"><a href=\"javascript:" 
        	+ this.varName + ".changeYear(1);\"><img src=\"" + this.imagePath 
        	+ "nextyear.gif\" alt=\"Next Year\" border=0></a></td>";
        middleWidth = "96%";
    }


    var html = new String();
    var src = (navigator.userAgent.indexOf("Netscape6") != -1)
            ? "src=\"javascript: void(0)\""
            : "src=\"" + this.imagePath + "spacer.gif\"";
    html += "<div id=\"iframeDiv\" style=\"position: absolute;left: 0px; z-index:" 
    	+ (CalendarZIndex + 1) + "; width=\"" + this.width 
    	+ "\"><iframe width=\"100%\" height=\"108\" frameborder=\"0\" " + src
    	+ " style=\"display: inherit\"></iframe></div>";
    html += "<div id=\"calendarDiv\" style=\"position: relative; top: 0px; z-index:" 
    	+ (CalendarZIndex + 2) + "\">";
    html += "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"" + this.width + "\"";
    html += " class=\"calBorder\">";
    
    html += "<tr><td>";
    html += "<table cellpadding=\"" + this.cellpadding + "\" cellspacing=\"" + this.cellspacing + "\" border=\"0\" width=\"" + this.width + "\">";

    html += "<tr><td colspan=\"7\" class=\"calTop\">";

    html += "	<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\">";

    html += "	<tr>";

    html += prevYear + "<td class=\"calTop\" align=\"left\" width=\"1%\"><a href=\"javascript:" + this.varName + ".changeMonth(-1);\"><img src=\"" + this.imagePath + "prevmonth.gif\" alt=\"Previous Month\" border=\"0\"></a></td>";

    html += "		<td class=\"calTop\" align=\"center\" width=\"" + middleWidth + "\"><img src=\"" + this.imagePath + "spacer.gif\" height=\"22\" width=\"1\" align=\"absmiddle\">";

    html += this.months[this.month];

    html += " " + this.year;

    html += "</td>";

    html += "		<td class=\"calTop\" align=\"right\" width=\"1%\"><a href=\"javascript:" + this.varName + ".changeMonth(1);\"><img src=\"" + this.imagePath + "nextmonth.gif\" alt=\"Next Month\" border=\"0\"></a></td>" + nextYear;

    html += "	</tr>";

    html += "	</table>";

    html += "</td></tr>";
    html += "<tr>";

    for( var i=0; i<this.days.length; i++ )
    {

        html += "<td class=\"calWeek\" align=\"center\">" + this.days[i] + "</td>";
    }

    html += "</tr>";

    html += this.writeDays();

    html += "</table></td></tr>";

    html += "</table>";
    html += "</div>";

    document.getElementById( this.divId ).innerHTML = html;
}


function clearSpaces( val )
{
    if(val != "")
    {
        var newVal = new String();

        for(var i=0; i<val.length; i++)
        {
            if(val.charAt(i) == " ")
                continue; // ignore leading space

            else
            {
                for(var j=i; j<val.length; j++)
                {
                    if(j == (val.length-1))
                    {
                        if(val.charAt(j) != " ")
                            newVal += val.charAt(j); // get last char if not a space
                        return newVal;
                    }

                    else if(newVal.charAt(j) == " " && newVal.charAt(j+1) == " ")
                        continue; // skip multiple spaces

                    else
                        newVal += val.charAt(j); // legal char added to new string
                }
            }
        }
        return newVal;
    }
    else
        return "";
}
