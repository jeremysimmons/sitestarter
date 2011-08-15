
				function createHttpRequest() {
					if (window.XMLHttpRequest) {
						return new XMLHttpRequest(); //Not IE
					} else if(window.ActiveXObject) {
						return new ActiveXObject("Microsoft.XMLHTTP"); //IE
					} else {
						alert("Your browser doesn't support the XmlHttpRequest object.  Please upgrade to Firefox.");
					}
				}