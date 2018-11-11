import * as React from "react";
import { resource } from '../config/Resource';
import { Defs, Stop, RadialGradient, } from 'react-native-svg-web';
export var Gradient = function () {
    return (React.createElement(Defs, null,
        React.createElement(RadialGradient, { id: "jovian", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "0%", stopColor: "#FFFFFF" }),
            React.createElement(Stop, { offset: "30%", stopColor: "#f6e9e0" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#600000" })),
        React.createElement(RadialGradient, { id: "iron", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "0%", stopColor: "#FFFFFF" }),
            React.createElement(Stop, { offset: "30%", stopColor: "#dddcdb" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#c9d3d8" })),
        React.createElement(RadialGradient, { id: "hotsuperearth", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "30%", stopColor: "#ffcc7f" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#fc6b11" })),
        React.createElement(RadialGradient, { id: "hotjupiter", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "0%", stopColor: "#FFFFFF" }),
            React.createElement(Stop, { offset: "30%", stopColor: "#f4af0e" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#f41d1d" })),
        React.createElement(RadialGradient, { id: "hotstone", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "30%", stopColor: "#ffcc7f" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#fc6b11" })),
        React.createElement(RadialGradient, { id: "stone", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "0%", stopColor: "#4286f4" }),
            React.createElement(Stop, { offset: "80%", stopColor: "#4286f4" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#2baf30" })),
        React.createElement(RadialGradient, { id: "coldstone", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "30%", stopColor: "#f2f2f7" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#8bb1ed" })),
        React.createElement(RadialGradient, { id: "neptunian", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "0%", stopColor: "#b0b0e8" }),
            React.createElement(Stop, { offset: "30%", stopColor: "#9595e2" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#a5a5d1" })),
        React.createElement(RadialGradient, { id: "superearth", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "30%", stopColor: "#4286f4" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#4286f4" })),
        React.createElement(RadialGradient, { id: "coldsuperearth", gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "30%", stopColor: "#f2f2f7" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#8bb1ed" })),
        React.createElement(RadialGradient, { id: "Star0", cx: "50%", cy: "0%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "#164dff" }),
            React.createElement(Stop, { offset: "60%", stopColor: "blue" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "StarTop0", cx: "50%", cy: "100%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "#164dff" }),
            React.createElement(Stop, { offset: "60%", stopColor: "blue" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "Star1", cx: "50%", cy: "0%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "white" }),
            React.createElement(Stop, { offset: "60%", stopColor: "#fffdd6" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "StarTop1", cx: "50%", cy: "100%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "white" }),
            React.createElement(Stop, { offset: "60%", stopColor: "#fffdd6" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "Star2", cx: "50%", cy: "0%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "orange" }),
            React.createElement(Stop, { offset: "60%", stopColor: "#ff6a00" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "StarTop2", cx: "50%", cy: "100%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "orange" }),
            React.createElement(Stop, { offset: "60%", stopColor: "#ff6a00" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "Star3", cx: "50%", cy: "0%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "red" }),
            React.createElement(Stop, { offset: "60%", stopColor: "#fc2c02" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "StarTop3", cx: "50%", cy: "100%", r: "50%" },
            React.createElement(Stop, { offset: "0%", stopColor: "red" }),
            React.createElement(Stop, { offset: "60%", stopColor: "#fc2c02" }),
            React.createElement(Stop, { offset: "100%", stopColor: "#black", stopOpacity: 0 })),
        React.createElement(RadialGradient, { id: "" + resource.color[3], gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "10%", stopColor: "#ff6a00" }),
            React.createElement(Stop, { offset: "50%", stopColor: "red" }),
            React.createElement(Stop, { offset: "80%", stopColor: "#50607a" })),
        React.createElement(RadialGradient, { id: "" + resource.color[2], gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "10%", stopColor: "#ff6a00" }),
            React.createElement(Stop, { offset: "50%", stopColor: "orange" }),
            React.createElement(Stop, { offset: "80%", stopColor: "#50607a" })),
        React.createElement(RadialGradient, { id: "" + resource.color[1], gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "10%", stopColor: "#fffdd6" }),
            React.createElement(Stop, { offset: "50%", stopColor: "white" }),
            React.createElement(Stop, { offset: "80%", stopColor: "#50607a" })),
        React.createElement(RadialGradient, { id: "" + resource.color[0], gradientUnits: "objectBoundingBox", fx: "30%", fy: "30%" },
            React.createElement(Stop, { offset: "10%", stopColor: "#164dff" }),
            React.createElement(Stop, { offset: "50%", stopColor: "blue" }),
            React.createElement(Stop, { offset: "80%", stopColor: "#50607a" }))));
};
