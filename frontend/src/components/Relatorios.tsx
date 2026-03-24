import React, { useState, useEffect } from 'react';
import { RelatorioTotaisPessoa, RelatorioTotaisCategoria } from '../types';
import api from '../services/api';

const Relatorios: React.FC = () => {
    const [relatorioPessoas, setRelatorioPessoas] = useState<RelatorioTotaisPessoa | null>(null);
    const [relatorioCategorias, setRelatorioCategorias] = useState<RelatorioTotaisCategoria | null>(null);
    const [carregando, setCarregando] = useState(true);
    const [erro, setErro] = useState<string>('');

    useEffect(() => {
        carregarRelatorios();
    }, []);

    const carregarRelatorios = async () => {
        try {
            setCarregando(true);
            const [dadosPessoas, dadosCategorias] = await Promise.all([
                api.obterRelatorioTotaisPorPessoa(),
                api.obterRelatorioTotaisPorCategoria()
            ]);
            setRelatorioPessoas(dadosPessoas);
            setRelatorioCategorias(dadosCategorias);
            setErro('');
        } catch (err) {
            setErro('Falha ao carregar relatórios');
            console.error(err);
        } finally {
            setCarregando(false);
        }
    };

    if (carregando) {
        return (
            <div className="flex justify-center items-center h-64">
                <div className="text-gray-500">Carregando relatórios...</div>
            </div>
        );
    }

    if (erro) {
        return (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
                {erro}
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Seção de Totais por Pessoa */}
            <div className="bg-white shadow rounded-lg p-6">
                <h2 className="text-2xl font-bold mb-6">Relatório de Totais por Pessoa</h2>
                
                {relatorioPessoas && relatorioPessoas.totaisPessoa.length === 0 ? (
                    <p className="text-gray-500">Nenhuma transação encontrada.</p>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Pessoa
                                    </th>
                                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Total Receitas
                                    </th>
                                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Total Despesas
                                    </th>
                                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Saldo
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {relatorioPessoas?.totaisPessoa.map((pessoa) => (
                                    <tr key={pessoa.pessoaId}>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                            {pessoa.nomePessoa}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-green-600">
                                            R$ {pessoa.totalReceita?.toFixed(2)}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-red-600">
                                            R$ {pessoa.totalDespesa?.toFixed(2)}
                                        </td>
                                        <td className={`px-6 py-4 whitespace-nowrap text-sm text-right font-semibold ${pessoa.saldo >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                                            R$ {pessoa.saldo?.toFixed(2)}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                            <tfoot className="bg-gray-100">
                                <tr>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900">
                                        TOTAL GERAL
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right font-bold text-green-600">
                                        R$ {relatorioPessoas?.resumo.receitaTotal?.toFixed(2)}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right font-bold text-red-600">
                                        R$ {relatorioPessoas?.resumo.despesaTotal?.toFixed(2)}
                                    </td>
                                    <td className={`px-6 py-4 whitespace-nowrap text-sm text-right font-bold ${(relatorioPessoas?.resumo.saldoLiquido || 0) >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                                        R$ {relatorioPessoas?.resumo.saldoLiquido?.toFixed(2)}
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                )}
            </div>
            
            {/* Seção de Totais por Categoria */}
            <div className="bg-white shadow rounded-lg p-6">
                <h2 className="text-2xl font-bold mb-6">Relatório de Totais por Categoria</h2>
                
                {relatorioCategorias && relatorioCategorias.totaisCategoria.length === 0 ? (
                    <p className="text-gray-500">Nenhuma transação encontrada.</p>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Categoria
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Finalidade
                                    </th>
                                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Total Receitas
                                    </th>
                                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Total Despesas
                                    </th>
                                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Saldo
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {relatorioCategorias?.totaisCategoria.map((categoria) => (
                                    <tr key={categoria.categoriaId}>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                            {categoria.descricaoCategoria}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                            {categoria.finalidadeCategoria}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-green-600">
                                            R$ {categoria.totalReceita?.toFixed(2)}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-red-600">
                                            R$ {categoria.totalDespesa?.toFixed(2)}
                                        </td>
                                        <td className={`px-6 py-4 whitespace-nowrap text-sm text-right font-semibold ${categoria.saldo >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                                            R$ {categoria.saldo?.toFixed(2)}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                            <tfoot className="bg-gray-100">
                                <tr>
                                    <td colSpan={2} className="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900">
                                        TOTAL GERAL
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right font-bold text-green-600">
                                        R$ {relatorioCategorias?.resumo.receitaTotal?.toFixed(2)}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right font-bold text-red-600">
                                        R$ {relatorioCategorias?.resumo.despesaTotal?.toFixed(2)}
                                    </td>
                                    <td className={`px-6 py-4 whitespace-nowrap text-sm text-right font-bold ${(relatorioCategorias?.resumo.saldoLiquido || 0) >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                                        R$ {relatorioCategorias?.resumo.saldoLiquido?.toFixed(2)}
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Relatorios;