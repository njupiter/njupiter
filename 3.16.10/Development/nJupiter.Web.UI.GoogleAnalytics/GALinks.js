
function GAControl_AltSites(arySites)
{
    init = function()
    {
         function linkGen(l,h)
         {
             if (!_ulink) return;
             var p,k,a="-",b="-",c="-",x="-",z="-",v="-";
             var dc=_ubd.cookie;
             if (!l || l=="") return;
             var iq = l.indexOf("?"); 
             var ih = l.indexOf("#"); 
             if (dc) {
              a=_uES(_uGC(dc,"__utma="+_udh,";"));
              b=_uES(_uGC(dc,"__utmb="+_udh,";"));
              c=_uES(_uGC(dc,"__utmc="+_udh,";"));
              x=_uES(_uGC(dc,"__utmx="+_udh,";"));
              z=_uES(_uGC(dc,"__utmz="+_udh,";"));
              v=_uES(_uGC(dc,"__utmv="+_udh,";"));
              k=(_uHash(a+b+c+x+z+v)*1)+(_udh*1);
              p="__utma="+a+"&__utmb="+b+"&__utmc="+c+"&__utmx="+x+"&__utmz="+z+"&__utmv="+v+"&__utmk="+k;
             }
             if (p) {
              if (h && ih>-1) return;
              if (h) { _udl.href=l+"#"+p; }
              else {
               if (iq==-1 && ih==-1) return (l+"?"+p);
               else if (ih==-1) return (l+"&"+p);
               else if (iq==-1) return (l.substring(0,ih-1)+"?"+p+l.substring(ih));
               else return (l.substring(0,ih-1)+"&"+p+l.substring(ih));
              }
             } else { alert(l); }
        }
        
        allLinks=document.getElementsByTagName('a');
        for(i=0;i<allLinks.length;i++)
        {
            for(x=0;x<arySites.length;x++)
            {
                if(allLinks[i].href.substring(0,arySites[x].length) == arySites[x] &&
                    location.href.substring(0,arySites[x].length) != arySites[x])
                {
                    allLinks[i].href = linkGen(allLinks[i].href);
                    break;
                }
            }
        }
        
        allForms=document.getElementsByTagName('form');
        for(i=0;i<allForms.length;i++)
        {
            for(x=0;x<arySites.length;x++)
            {
                if(allForms[i].action.substring(0,arySites[x].length) == arySites[x] &&
                    location.href.substring(0,arySites[x].length) != arySites[x])
                {
                    allForms[i].action = linkGen(allForms[i].action);
                    break;
                }
            }
        }
    }
    if(window.addEventListener)
        window.addEventListener('load',init,false);
    else
        window.attachEvent('onload',init);
}

function GAControl_SetCampaign(source,medium,term,content,campaign)
{
    _uGC_b = window._uGC;
    _uGC = function(l, n, s) {
    if (n=="utm_source=")
    return source;
    else if (n=="utm_medium=")
    return medium;
    else if (n=="utm_term=")
    return term;
    else if (n=="utm_content=")
    return content;
    else if (n=="utm_campaign=")
    return campaign;
    else
    return _uGC_b(l,n,s);
    }
}


function GAControl_SetTrans(trans) {

    function init()
    {  
         var l=trans.split("UTM:");
         var i,i2,c;
         if (_userv==0 || _userv==2) i=new Array();
         if (_userv==1 || _userv==2) { i2=new Array(); c=_uGCS(); }

         for (var ii=0;ii<l.length;ii++) {
          l[ii]=_uTrim(l[ii]);
          if (l[ii].charAt(0)!='T' && l[ii].charAt(0)!='I') continue;
          var r=Math.round(Math.random()*2147483647);
          if (!_utsp || _utsp=="") _utsp="|";
          var f=l[ii].split(_utsp),s="";
          if (f[0].charAt(0)=='T') {
           s="&utmt=tran"+"&utmn="+r;
           f[1]=_uTrim(f[1]); if(f[1]&&f[1]!="") s+="&utmtid="+_uES(f[1]);
           f[2]=_uTrim(f[2]); if(f[2]&&f[2]!="") s+="&utmtst="+_uES(f[2]);
           f[3]=_uTrim(f[3]); if(f[3]&&f[3]!="") s+="&utmtto="+_uES(f[3]);
           f[4]=_uTrim(f[4]); if(f[4]&&f[4]!="") s+="&utmttx="+_uES(f[4]);
           f[5]=_uTrim(f[5]); if(f[5]&&f[5]!="") s+="&utmtsp="+_uES(f[5]);
           f[6]=_uTrim(f[6]); if(f[6]&&f[6]!="") s+="&utmtci="+_uES(f[6]);
           f[7]=_uTrim(f[7]); if(f[7]&&f[7]!="") s+="&utmtrg="+_uES(f[7]);
           f[8]=_uTrim(f[8]); if(f[8]&&f[8]!="") s+="&utmtco="+_uES(f[8]);
          } else {
           s="&utmt=item"+"&utmn="+r;
           f[1]=_uTrim(f[1]); if(f[1]&&f[1]!="") s+="&utmtid="+_uES(f[1]);
           f[2]=_uTrim(f[2]); if(f[2]&&f[2]!="") s+="&utmipc="+_uES(f[2]);
           f[3]=_uTrim(f[3]); if(f[3]&&f[3]!="") s+="&utmipn="+_uES(f[3]);
           f[4]=_uTrim(f[4]); if(f[4]&&f[4]!="") s+="&utmiva="+_uES(f[4]);
           f[5]=_uTrim(f[5]); if(f[5]&&f[5]!="") s+="&utmipr="+_uES(f[5]);
           f[6]=_uTrim(f[6]); if(f[6]&&f[6]!="") s+="&utmiqt="+_uES(f[6]);
          }
          if ((_userv==0 || _userv==2) && _uSP()) {
           i[ii]=new Image(1,1);
           i[ii].src=_ugifpath+"?"+"utmwv="+_uwv+s;
           i[ii].onload=function() { _uVoid(); }
          }
          if ((_userv==1 || _userv==2) && _uSP()) {
           i2[ii]=new Image(1,1);
           i2[ii].src=_ugifpath2+"?"+"utmwv="+_uwv+s+"&utmac="+_uacct+"&utmcc="+c;
           i2[ii].onload=function() { _uVoid(); }
          }
         }
         return;
    }
    
    if(window.addEventListener)
        window.addEventListener('load',init,false);
    else
        window.attachEvent('onload',init);
        
}