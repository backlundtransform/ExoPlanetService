var path = require('path');

module.exports = {
    entry: {
        main: './wwwroot/js/main.js'
    },
    output: {
       publicPath: "/js/",
       path: path.join(__dirname, '/wwwroot/js/'),
       filename: 'main.build.js'
    },
    module: {
        rules: [
       
          {
            test: /\.css/,
            loaders:  'style-loader!css-loader',
          
          },  {
            test: /\.(gif|png|jpe?g|svg)$/i,
            loaders:  'file-loader',
          
          },  {
            test:  /\.js$/,
            loaders:  'babel-loader',
          
          }
        ],
      }
};