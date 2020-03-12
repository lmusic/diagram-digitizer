import React, {Component} from 'react';
import axios from "axios";
import Plotly from 'plotly.js';
import './App.css';
class App extends Component{
    state = {
        horizontalFile: null,
        verticalFile: null,
        horizontalUrl: null,
        verticalUrl: null
    };

      render () {
          return (
              <div className="App">
                  <div className="container">
                      <div className="left">
                          <p>Horizontal: </p>
                          <input onChange={this.horizontalFileSelectedHandler} type='file'/> <br/>
                          <img className={'w-300'} src={this.state.horizontalUrl} alt=""/>
                      </div>
                      <div className="w-400">
                          <div id="horizontal"></div>
                      </div>
                  </div>
                    <div className="container">
                        <div className="w-300">
                            <p>Vertical: </p>
                            <input onChange={this.verticalFileSelectedHandler} type='file'/> <br/>
                            <img className={'w-300'} src={this.state.verticalUrl} alt=""/> <br/>
                        </div>
                        <div className="w-400">
                            <div id="vertical"></div>
                        </div>
                    </div>
                  <button onClick={this.fileUploadHandler}>UPLOAD</button>
              </div>
          )
      }

      // componentDidMount() {
      //     this.drawResult(1);
      // }

    horizontalFileSelectedHandler = event => {
        const fr = new FileReader();
        const file = event.target.files[0];
        fr.addEventListener('load', () => {
            this.setState({
                horizontalFile: file,
                horizontalUrl: fr.result,
                verticalFile: this.state.verticalFile,
                verticalUrl: this.state.verticalUrl
            });
        });
        fr.readAsDataURL(file);
      };

    verticalFileSelectedHandler = event => {
        const fr = new FileReader();
        const file = event.target.files[0];
        fr.addEventListener('load', () => {
            this.setState({
                verticalFile: file,
                verticalUrl: fr.result,
                horizontalFile: this.state.horizontalFile,
                horizontalUrl: this.state.horizontalUrl
            });
        });
        fr.readAsDataURL(file);
    };

      fileUploadHandler = () => {
          let fd = new FormData();
          fd.append('horizontal',this.state.horizontalFile);
          fd.append('vertical', this.state.verticalFile);
        axios.post('https://localhost:44331/api/v1/image/upload', fd)
            .then(res => {
                this.drawVertical(res.data.vertical);
                this.drawHorizontal(res.data.horizontal);
                // axios.get('https://localhost:44331/api/v1/image/download')
                //     .then(res => {
                //         const blob = new Blob([res.data], {type: res.headers['Content-Type']});
                //         const link = document.createElement('a');
                //         link.href = window.URL.createObjectURL(blob);
                //         link.download = 'result.msi';
                //         link.click();
                //     });
            })
            .catch(res => {
                console.log(res);
            })
      };

      drawVertical(radArray) {
              let angles = [];
              for (let i = 0; i < 360; i++) {
                  angles.push(i);
              }
              let changedRad = radArray.map((item)=> {
                  return -1*item;
              });
              var trace1 = {
                  r: changedRad,
                  theta: angles,
                  mode: 'lines',
                  name: 'line',
                  line: {color: 'blue'},
                  type: 'scatterpolar'
              };

              var data = [trace1];

              var layout = {
                  title: 'Vertical',
                  font: {
                      family: 'Arial, sans-serif;',
                      size: 10,
                      color: '#000'
                  },
                  showlegend: false,
                  orientation: -90
              };
              Plotly.newPlot('vertical', data, layout);
      }

      drawHorizontal(radArray){
          let angles = [];
          for (let i = 0; i < 360; i++) {
              angles.push(i);
          }
          let changedRad = radArray.map((item)=> {
              return -1*item;
          });
          var trace1 = {
              r: changedRad,
              theta: angles,
              mode: 'lines',
              name: 'line',
              line: {color: 'blue'},
              type: 'scatterpolar'
          };

          var data = [trace1];

          var layout = {
              title: 'Mic Patterns',
              font: {
                  family: 'Arial, sans-serif;',
                  size: 10,
                  color: '#000'
              },
              showlegend: false,
              orientation: -90
          };
          Plotly.newPlot('horizontal', data, layout);
      }
}

export default App;
