import * as React from 'react';
import Icon from 'react-native-vector-icons/FontAwesome';
import styles from '../styles/defaultStyle'
import{resource} from '../config/Resource'
import Svg,{
  Circle,
  Ellipse,
  Pattern,
  Path,
Defs,
G,Use

} from 'react-native-svg';
const habmenu= [{
    icon:<Svg  width={30}   height={30} viewBox="0 0 24 24">
    <Path fill="#c6d4ff" d="M12,3C16.97,3 21,6.58 21,11C21,15.42 15,21 12,21C9,21 3,15.42 3,11C3,6.58 7.03,3 12,3M10.31,10.93C9.29,9.29 7.47,8.58 6.25,9.34C5.03,10.1 4.87,12.05 5.89,13.69C6.92,15.33 8.74,16.04 9.96,15.28C11.18,14.5 11.33,12.57 10.31,10.93M13.69,10.93C12.67,12.57 12.82,14.5 14.04,15.28C15.26,16.04 17.08,15.33 18.11,13.69C19.13,12.05 18.97,10.1 17.75,9.34C16.53,8.58 14.71,9.29 13.69,10.93M12,17.75C10,17.75 9.5,17 9.5,17C9.5,17.03 10,19 12,19C14,19 14.5,17 14.5,17C14.5,17 14,17.75 12,17.75Z" />
     </Svg>,
     name:resource.habmenu[0],
     filter:{Key: 'Hab'} 
    },
    {
       icon:<Icon style={styles.habicon} name ={"thermometer-empty"}  />,
       name:resource.habmenu[1],
       filter:{Key: 'Temp', MinValue:-100, MaxValue:-50} 
      
    },
    {
      icon:<Icon style={styles.habicon} name ={"thermometer-half"}  />,
      name:resource.habmenu[2],
      filter:{Key: 'Temp', MinValue:-50, MaxValue:50} 
    },
    {
      icon:<Icon style={styles.habicon} name ={"thermometer"}  />,
      name:resource.habmenu[3],
      filter:{Key: 'Temp', MinValue:50, MaxValue:100} 
    },
    {
      icon:<Icon style={styles.habicon} name ={"globe"}  />,
      name:resource.habmenu[4],
      filter:{Key: 'Esi', MinValue:0.6, MaxValue:1} 
    },
    {
       icon:<Icon style={styles.habicon} name ={"leaf"}  />,
       name:resource.habmenu[5],
       filter:{Key: 'Sph', MinValue:0.6, MaxValue:1} 
    },
    {
       icon:<Icon style={styles.habicon} name ={"moon-o"}  />,
       name:resource.habmenu[6],
       filter:{Key: 'Moons' } 
     }]

     const constellations = resource.const.map((obj,index) =>{ 
      var rObj = {  icon:<Svg preserveAspectRatio="xMidYMid meet" viewBox="0 0 640 640"  width="100" height="100"></Svg>,
      name:obj,
      id:index};

      return rObj;
   });
     
  

export const constants ={
tiles:"/Content/tiles/{z}/tile.jpg",
habmenu,
constellations

}