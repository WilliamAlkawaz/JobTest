import {useState} from 'react'

const AddCategory = ({ ranges, addCategory }) => {
    const[name,setName] = useState('');
    const[imSource,setImSource] = useState('');  
    const imSourceChanged = imS => {
        setImSource(imS);       
    }

    const onAddDown = () => {
        if(!name)
        {
            alert('Please enter name');
            return;
        }

        if (!imSource) {
            alert('Pease upload icon');
            return;
        }

        let min = ranges[ranges.length-2]+Math.round((ranges[ranges.length-1]-ranges[ranges.length-2])/2);
        let max = ranges[ranges.length-1];
        ranges.push(max);
        ranges[ranges.length - 2] = min;   
        let im = document.getElementById('file');
        addCategory({name,min,max,imSource}, im);
        
        setName('');
        setImSource('');
    }

    return (
        <div className="add-form">
            <div className='form-control'>
                <label>Name</label>
                <input type='text' placeholder='Category name' value={name} onChange={(e)=>setName(e.target.value)} />
            </div>
            <div className='form-control'>
                <img width='80' src={imSource} alt='Please upload icon' />
                <label>Upload icon image</label>
                <input type='file' id='file'
                    onChange={(imURL) => {
                        var file = document.getElementById('file');
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            imSourceChanged(e.target.result);
                        };
                        reader.onerror = function (e) {
                            console.log("cannot read file");
                        };
                        reader.readAsDataURL(file.files[0]);
                    }}
                />
            </div>
            
            <input type='button' value='Save Category' className='btn btn-block' onClick={onAddDown}/>
        </div>
    )
}

export default AddCategory
