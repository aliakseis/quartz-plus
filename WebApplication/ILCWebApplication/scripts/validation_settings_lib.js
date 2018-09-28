function WebForm_RemoveClassName(element, className) {
    var current = element.className;
    if (current) {
        if (current.substring(current.length - className.length - 1, current.length) == ' ' + className) {
            element.className = current.substring(0, current.length - className.length - 1);
            return;
        }
        // Fix for missing ending space case
        if ((current + ' ').substring(current.length - className.length, current.length + 1) == ' ' + className) {
            element.className = current.substring(0, current.length - className.length);
            return;
        }
        if (current == className) {
            element.className = "";
            return;
        }
        // Fix for missing ending space case
        if (current + ' ' == className) {
            element.className = "";
            return;
        }
        var index = current.indexOf(' ' + className + ' ');
        if (index != -1) {
            element.className = current.substring(0, index) + current.substring(index + className.length + 2, current.length);
            return;
        }
        if (current.substring(0, className.length) == className + ' ') {
            element.className = current.substring(className.length + 1, current.length);
        }
    }
}

function getPrevScroll() {
	var re = new RegExp("scrollPos([^=]+)=([0-9]+)", "g");
	var cookieStr = document.cookie;
	while ((m = re.exec(cookieStr)) != null) {
	    var obj = window.frames[m[1]];
	    if (obj && obj.scrollTo)
	        obj.scrollTo(0, m[2]);
	    else {
	        obj = document.getElementById(m[1]);
	        if (obj)
	            obj.scrollTop = m[2];
	    }
    }
}
