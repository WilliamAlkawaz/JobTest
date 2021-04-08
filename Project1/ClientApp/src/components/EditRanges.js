import React from "react";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import { Slider, Rail, Handles, Tracks, Ticks } from "react-compound-slider";
import { MuiRail, MuiHandle, MuiTrack, MuiTick } from "./components"; // example render components - source below

// Based on Material Design spec:
// Styles by https://github.com/RafeSacks
// https://github.com/sghall/react-compound-slider/issues/41
// https://material.io/design/components/sliders.html#spec

class EditRanges extends React.Component {
  constructor(props) {
    super(props);

    const initialValues = props.ranges;

    this.state = {
      domain: [props.ranges[0], props.ranges[props.ranges.length-1]],
      values: [...initialValues],
      update: [...initialValues]
    };
  }

  onUpdate = (update) => {
    this.setState({ update });
  };

  onChange = (values) => {
    this.setState({ values });
  };

  onAddDown = () => {
    this.props.onEdit(this.state.values); 
    console.log('save clicked');
  };

  render() {
    const { domain, values, update } = this.state;

    return (
      <Grid container>
        <Grid item xs={12}>
          <Typography>Update:</Typography>
          <Typography>{update.join("🍔")}</Typography>
          <Typography>Values:</Typography>
          <Typography>{values.join("🍔")}</Typography>
          <div style={{ margin: "10%", height: 120, width: "80%" }}>
            <Slider
              mode={2}
              step={1}
              domain={domain}
              rootStyle={{
                position: "relative",
                width: "100%"
              }}
              onUpdate={this.onUpdate}
              onChange={this.onChange}
              values={values}
            >
              <Rail>
                {({ getRailProps }) => <MuiRail getRailProps={getRailProps} />}
              </Rail>
              <Handles>
                {({ handles, getHandleProps }) => (
                  <div className="slider-handles">
                    {handles.map((handle) => (
                      <MuiHandle
                        key={handle.id}
                        handle={handle}
                        domain={domain}
                        getHandleProps={getHandleProps}
                      />
                    ))}
                  </div>
                )}
              </Handles>
              <Tracks left={false} right={false}>
                {({ tracks, getTrackProps }) => (
                  <div className="slider-tracks">
                    {tracks.map(({ id, source, target }) => (
                      <MuiTrack
                        key={id}
                        source={source}
                        target={target}
                        getTrackProps={getTrackProps}
                      />
                    ))}
                  </div>
                )}
              </Tracks>
              <Ticks count={5}>
                {({ ticks }) => (
                  <div className="slider-ticks">
                    {ticks.map((tick) => (
                      <MuiTick key={tick.id} tick={tick} count={ticks.length} />
                    ))}
                  </div>
                )}
              </Ticks>
            </Slider>
          </div>
        </Grid>
        <input type='button' value='Save Category' className='btn btn-block' onClick={this.onAddDown}/>
      </Grid>
    );
  }
}

export default EditRanges
