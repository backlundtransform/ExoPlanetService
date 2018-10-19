var path = require('path');

module.exports = {
  watch:true,
    entry: {
        main: './wwwroot/js/main.js',
        react: './ClientApp/src/App.js'

    },
    output: {
       publicPath: "/js/",
       path: path.join(__dirname, '/wwwroot/js/'),
        filename: '[name].build.js'
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
            options:{

              presets:['babel-preset-env', 'babel-preset-react']
            }
         
          
          }
        ],
      }
};