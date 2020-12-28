const fs = require('fs');
const webpack = require('webpack');

const configs = [];
fs.readdirSync("./src/workers/").forEach(function (file) {
  configs.push({
    entry: `./src/workers/${file}`,
    output: {filename: `../public/workers/${file}`},
    module: {
      rules: [
        {
          test: /\.m?js$/,
          exclude: /(node_modules|bower_components)/,
          use: {
            loader: 'babel-loader',
            options: {
              presets: ['@babel/preset-env']
            }
          }
        }
      ]
    }
  });
});

webpack(
  configs,
  (err, stats) => {
    if (err || stats.hasErrors()) {
      console.log(err);
    }
    fs.rmdirSync("./dist/");
  }
);
