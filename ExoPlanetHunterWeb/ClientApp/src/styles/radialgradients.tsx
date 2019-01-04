import  * as React from "react";

import{resource} from '../config/Resource'

import {
    Defs,
  Stop,
  RadialGradient,
} from 'react-native-svg-web';
import { Star } from "../service/getPlanets";


export const Gradient =()=>{ 
  
  return(<Defs><RadialGradient id={"jovian"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="0%" stopColor="#FFFFFF" />
<Stop offset="30%" stopColor="#f6e9e0" />
<Stop offset="100%" stopColor="#600000" />
</RadialGradient>
<RadialGradient id={"iron"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="0%" stopColor="#FFFFFF"  />
<Stop offset="30%" stopColor="#dddcdb" />
<Stop offset="100%" stopColor="#c9d3d8" />
</RadialGradient>
<RadialGradient id={"hotsuperearth"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">

<Stop offset="30%" stopColor="#ffcc7f" />
<Stop offset="100%" stopColor="#fc6b11"/>
</RadialGradient>
<RadialGradient id={"hotjupiter"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="0%" stopColor="#FFFFFF" />
<Stop offset="30%" stopColor="#f4af0e" />
<Stop offset="100%" stopColor="#f41d1d" />
</RadialGradient>
<RadialGradient id={"hotstone"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="30%" stopColor="#ffcc7f" />
<Stop offset="100%" stopColor="#fc6b11"/>
</RadialGradient>
<RadialGradient id={"stone"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="0%" stopColor="#0047ba" />
<Stop offset="80%" stopColor="#4286f4" />
<Stop offset="100%" stopColor="#4286f4"/>
</RadialGradient>
<RadialGradient id={"coldstone"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="30%" stopColor="#f2f2f7"/>
<Stop offset="100%" stopColor="#8bb1ed" />

</RadialGradient>
<RadialGradient id={"neptunian"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="0%" stopColor="#b0b0e8" />
<Stop offset="30%" stopColor="#9595e2" />
<Stop offset="100%" stopColor="#a5a5d1" />
</RadialGradient>
<RadialGradient id={"superearth"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">

<Stop offset="30%" stopColor="#4286f4" />
<Stop offset="100%" stopColor="#4286f4"/>
 </RadialGradient>
<RadialGradient id={"coldsuperearth"}
gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="30%" stopColor="#f2f2f7"/>
<Stop offset="100%" stopColor="#8bb1ed" />

</RadialGradient>
<RadialGradient id={`Star0`} cx="50%" cy="0%" r="50%" > 
<Stop offset="0%"  stopColor="#164dff" />
<Stop offset="60%"  stopColor="blue"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 
<RadialGradient id={`StarTop0`} cx="50%" cy="100%" r="50%" > 
<Stop offset="0%"  stopColor="#164dff" />
<Stop offset="60%"  stopColor="blue"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 
<RadialGradient id={`Star1`} cx="50%" cy="0%" r="50%" > 
<Stop offset="0%"  stopColor="white" />
<Stop offset="60%"  stopColor="#fffdd6"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 
<RadialGradient id={`StarTop1`} cx="50%" cy="100%" r="50%" > 
<Stop offset="0%"  stopColor="white" />
<Stop offset="60%"  stopColor="#fffdd6"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 
<RadialGradient id={`Star2`} cx="50%" cy="0%" r="50%" > 
<Stop offset="0%"  stopColor="orange" />
<Stop offset="60%"  stopColor="#ff6a00"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 
<RadialGradient id={`StarTop2`} cx="50%" cy="100%" r="50%" > 
<Stop offset="0%"  stopColor="orange" />
<Stop offset="60%"  stopColor="#ff6a00"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 
<RadialGradient id={`Star3`} cx="50%" cy="0%" r="50%" > 
<Stop offset="0%"  stopColor="red" />
<Stop offset="60%"  stopColor="#fc2c02"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 
<RadialGradient id={`StarTop3`} cx="50%" cy="100%" r="50%" > 
<Stop offset="0%"  stopColor="red" />
<Stop offset="60%"  stopColor="#fc2c02"  />
<Stop offset="100%" stopColor="#black" stopOpacity={0}  />
</RadialGradient> 

<RadialGradient id={`b3`}    gradientUnits="objectBoundingBox">
<Stop offset="30%" stopColor="red"/>
<Stop offset="80%" stopColor="#fc2c02"/>
</ RadialGradient>
<RadialGradient id={`b2`}    gradientUnits="objectBoundingBox">
<Stop offset="30%" stopColor="orange"/>
<Stop offset="80%" stopColor="#ff6a00"/>
</ RadialGradient>
<RadialGradient id={`b1`}    gradientUnits="objectBoundingBox">
<Stop offset="30%" stopColor="white"/>
<Stop offset="80%" stopColor="#fffdd6"/>
</ RadialGradient>
<RadialGradient id={`b0`}    gradientUnits="objectBoundingBox">
<Stop offset="30%" stopColor="#164dff"/>
<Stop offset="80%" stopColor="blue"/>
</ RadialGradient>
 </Defs>) }
    
