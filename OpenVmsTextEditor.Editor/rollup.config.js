import commonjs from '@rollup/plugin-commonjs';
import { nodeResolve } from '@rollup/plugin-node-resolve';

export default {
    input: 'out/openVmsEditor.js',
    output: {
        name: "openVmsEditor",
        format: 'iife',
        file: "../OpenVmsTextEditor.Web/OpenVmsTextEditor.Web/wwwroot/js/openVmsEditor.bundle.js"
    },
    plugins: [
        nodeResolve(),
        commonjs()
    ]
}