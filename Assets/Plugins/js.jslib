mergeInto(LibraryManager.library, {

  IsMobile: function (){
        var userAgentInfo = navigator.userAgent;
   			var Agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];
   			var flag = false;
   				for (var v = 0; v < Agents.length; v++)
   					{
   						  if (userAgentInfo.indexOf(Agents[v]) > 0)
   							{
   								  flag = true;
   			            break;
   							}
   					}
   				return flag;
  },

  OpenPage: function(url){
	  var Fixedurl = Pointer_stringify(url);
	  console.log('Open link' +Fixedurl);
	  window.open(Fixedurl,'_blank');
  },

  ClosePage: function(){
	console.log('CloseWindow');
	if(navigator.userAgent.indexOf("Firefox") != -1 || navigator.userAgent.indexOf("Chrome") != -1)
	{
        window.location.href = "about:blank";
        window.close();
    }
	else
	{
        window.opener = null;
        window.open("", "_self");
        window.close();
    }
  },

});