var path = require('path');

module.exports = {
  watch:true,
    entry: {
        main: './wwwroot/js/main.js',
        react: './ClientApp/src/App.tsx'

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
            test: /\.(gif|png|eot|woff2|woff|woff2|ttf|jpe?g|svg)$/i,
            loaders:  'file-loader',
          
          },  {
            test:  /\.js$/,
          
            loaders:  'babel-loader',
            include: ['/node_modules/@amcharts'],
            options:{

              presets:['babel-preset-env', 'babel-preset-react']
            }
         
          
          }, {
            test: /\.tsx?$/,
            use: [
              {
                loader: 'ts-loader',
                options: {
                  transpileOnly: true,
                  experimentalWatchApi: true,
                },
              },
            ],
            exclude: /node_modules/
          }
        ],
      },
      resolve: {
        extensions: [ '.tsx', '.ts', '.js' ],
        alias: {
          'react-native-svg': 'react-native-svg-web'
        }
      }
};